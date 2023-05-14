using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Domain.Models;
using SFA.DAS.Employer.Profiles.Web.Controllers;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;

namespace SFA.DAS.Employer.Profiles.Web.UnitTests.Controllers;

public class UserControllerTests
{
    [Test, MoqAutoData]
    public void Then_The_View_Is_Returned_With_Model(
        [Frozen] Mock<IConfiguration> configuration,
        [Greedy] UserController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");

        var actual = controller.ChangeSignInDetails() as ViewResult;

        actual.Should().NotBeNull();
        var actualModel = actual?.Model as ChangeSignInDetailsViewModel;
        Assert.AreEqual("https://home.integration.account.gov.uk/settings", actualModel?.SettingsLink);
    }

    [Test, MoqAutoData]
    public async Task When_InValid_Model_And_Auth_Is_Given_Throw_Invalid_ModelState(
        string emailClaimValue,
        string nameClaimValue,
        string errorMessage,
        AddUserDetailsModel model,
        [Frozen] Mock<IConfiguration> configuration,
        [Greedy] UserController controller)
    {
        // arrange
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, emailClaimValue),
                new Claim(ClaimTypes.NameIdentifier, nameClaimValue)
            })})
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        controller.ModelState.AddModelError(nameof(model.FirstName), errorMessage);
        controller.ModelState.AddModelError(nameof(model.LastName), errorMessage);


        // sut
        var actual = (ViewResult) await controller.AddUserDetails(model);

        // assert
        actual.Should().NotBeNull();
        var actualModel = actual.Model as AddUserDetailsModel;
        actualModel.FirstNameError.Length.Should().BeGreaterThanOrEqualTo(1);
        actualModel.LastNameError.Length.Should().BeGreaterThanOrEqualTo(1);
    }

    [Test, MoqAutoData]
    public async Task When_Valid_Model_And_Auth_Is_Given_AccountService_Return_Redirect(
        string emailClaimValue,
        string nameClaimValue,
        string userId,
        string firstName,
        string lastName,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IEmployerAccountService> employerAccountService,
        [Greedy] UserController controller)
    {
        // arrange
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        var model = new AddUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
        };

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, emailClaimValue),
                new Claim(ClaimTypes.NameIdentifier, nameClaimValue),
                new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, userId)
            })})
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        
        // sut
        var actual = await controller.AddUserDetails(model);

        // assert
        if (actual is RedirectResult result)
            _ = result.Url.Should().BeEquivalentTo("https://accounts.test-eas.apprenticeships.education.gov.uk/service/index");
        employerAccountService.Verify(x=>x.UpsertUserAccount(userId, 
            It.Is<UpsertAccountRequest>(c=>
                c.GovIdentifier.Equals(nameClaimValue)
                && c.FirstName.Equals(model.FirstName)
                && c.LastName.Equals(model.LastName)
                )), Times.Once);
    }
}