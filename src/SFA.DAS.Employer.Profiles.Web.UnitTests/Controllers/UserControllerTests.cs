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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

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
    public void Then_The_View_Is_Returned_With_Model_With_No_Account(
        [Frozen] Mock<IConfiguration> configuration,
        [Greedy] UserController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");

        var actual = controller.ChangeSignInDetailsNoAccount() as ViewResult;

        actual.Should().NotBeNull();
        var actualModel = actual?.Model as ChangeSignInDetailsViewModel;
        Assert.AreEqual("https://home.integration.account.gov.uk/settings", actualModel?.SettingsLink);
    }

    [Test, MoqAutoData]
    public void Then_The_FirstName_LastName_And_CorrelationId_Are_Passed_To_The_View(
        string firstName,
        string lastName,
        string correlationId,
        [Frozen] Mock<IConfiguration> configuration,
        [Greedy] UserController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("prd");

        var actual = controller.AddUserDetails(firstName, lastName, correlationId);
        
        Assert.IsNotNull(actual);
        var actualViewResult = actual as ViewResult;
        Assert.IsNotNull(actualViewResult);
        var actualModel = actualViewResult!.Model as AddUserDetailsModel;
        Assert.IsNotNull(actualModel);
        actualModel!.FirstName.Should().Be(firstName);
        actualModel!.LastName.Should().Be(lastName);
        actualModel!.CorrelationId.Should().Be(correlationId);
        actualModel.TermsOfUseLink.Should().Be("https://accounts.manage-apprenticeships.service.gov.uk/service/termsAndConditions/overview");
    }
    
    [Test, MoqAutoData]
    public void Then_The_TermsAndConditionsUrl_Is_Correctly_Generated_Are_Passed_To_The_View(
        [Frozen] Mock<IConfiguration> configuration,
        [Greedy] UserController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");

        var actual = controller.AddUserDetails();
        
        Assert.IsNotNull(actual);
        var actualViewResult = actual as ViewResult;
        Assert.IsNotNull(actualViewResult);
        var actualModel = actualViewResult!.Model as AddUserDetailsModel;
        Assert.IsNotNull(actualModel);
        actualModel!.FirstName.Should().BeNullOrEmpty();
        actualModel!.LastName.Should().BeNullOrEmpty();
        actualModel!.CorrelationId.Should().BeNullOrEmpty();
        actualModel.TermsOfUseLink.Should().Be("https://accounts.test-eas.apprenticeships.education.gov.uk/service/termsAndConditions/overview");
    }

    [Test, MoqAutoData]
    public void When_InValid_Model_And_Auth_Is_Given_Throw_Invalid_ModelState(
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
        var actual = (ViewResult)controller.AddUserDetails(model);

        // assert
        actual.Should().NotBeNull();
        var actualModel = actual.Model as AddUserDetailsModel;
        actualModel.FirstNameError.Length.Should().BeGreaterThanOrEqualTo(1);
        actualModel.LastNameError.Length.Should().BeGreaterThanOrEqualTo(1);
    }

    [Test, MoqAutoData]
    public void When_Valid_Model_And_Auth_Is_Given_AccountService_Called_Claims_Added_And_Return_Redirect_Confirm(
        string emailClaimValue,
        string nameClaimValue,
        string userId,
        string firstName,
        string lastName,
        [Frozen] Mock<IUrlHelperFactory> urlHelperFactory,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IAuthenticationService> authenticationService,
        [Frozen] Mock<IServiceProvider> serviceProviderMock,
        [NoAutoProperties] UserController controller)
    {
        // arrange
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        var model = new AddUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
        };
        serviceProviderMock
            .Setup(_ => _.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationService.Object);

        serviceProviderMock
            .Setup(x => x.GetService(typeof(IUrlHelperFactory)))
            .Returns(urlHelperFactory.Object);

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, emailClaimValue),
                new Claim(ClaimTypes.NameIdentifier, nameClaimValue),
                new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, userId)
            })}),
            RequestServices = serviceProviderMock.Object
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        
        // sut
        var actual = controller.AddUserDetails(model) as RedirectToRouteResult;

        // assert
        actual.RouteName.Should().Be(RouteNames.ConfirmUserDetails);
    }

    [Test, MoqAutoData]
    public async Task When_Valid_Confirmation_Model_And_Auth_Is_Given_AccountService_Called_Claims_Added_And_Return_Redirect_To_Employer_Accounts_When_No_CorrelationId(
        string emailClaimValue,
        string nameClaimValue,
        string userId,
        string firstName,
        string lastName,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IEmployerAccountService> employerAccountService,
        [Frozen] Mock<IAuthenticationService> authenticationService,
        [Frozen] Mock<IServiceProvider> serviceProviderMock,
        [Greedy] UserController controller)
    {
        // arrange
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        var model = new AddUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
        };
        serviceProviderMock
            .Setup(_ => _.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationService.Object);

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, emailClaimValue),
                new Claim(ClaimTypes.NameIdentifier, nameClaimValue),
                new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, userId)
            })}),
            RequestServices = serviceProviderMock.Object
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // sut
        var actual = await controller.ConfirmUserDetails(model);

        // assert
        if (actual is RedirectResult result)
            _ = result.Url.Should().BeEquivalentTo("https://accounts.test-eas.apprenticeships.education.gov.uk/service/index");
        employerAccountService.Verify(x => x.UpsertUserAccount(userId,
            It.Is<UpsertAccountRequest>(c =>
                c.GovIdentifier.Equals(nameClaimValue)
                && c.FirstName.Equals(model.FirstName)
                && c.LastName.Equals(model.LastName)
                )), Times.Once);
        httpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.GivenName)).Value.Should().Be(model.FirstName);
        httpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.FamilyName)).Value.Should().Be(model.LastName);
        httpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier)).Value.Should().Be(emailClaimValue);
        authenticationService.Verify(x => x.SignInAsync(httpContext, CookieAuthenticationDefaults.AuthenticationScheme, httpContext.User, null));
    }

    [Test, MoqAutoData]
    public async Task When_Valid_Confirmation_Model_And_Auth_Is_Given_AccountService_Called_Claims_Added_And_Return_Redirect_To_RegisterNew_In_Employer_Accounts_When_CorrelationId(
        string emailClaimValue,
        string nameClaimValue,
        string userId,
        string firstName,
        string lastName,
        Guid correlationId,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IEmployerAccountService> employerAccountService,
        [Frozen] Mock<IAuthenticationService> authenticationService,
        [Frozen] Mock<IServiceProvider> serviceProviderMock,
        [Greedy] UserController controller)
    {
        // arrange
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        var model = new AddUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
            CorrelationId = correlationId.ToString()
        };
        serviceProviderMock
            .Setup(_ => _.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationService.Object);

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, emailClaimValue),
                new Claim(ClaimTypes.NameIdentifier, nameClaimValue),
                new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, userId)
            })}),
            RequestServices = serviceProviderMock.Object
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // sut
        var actual = await controller.ConfirmUserDetails(model);

        // assert
        if (actual is RedirectResult result)
            _ = result.Url.Should().BeEquivalentTo($"https://accounts.test-eas.apprenticeships.education.gov.uk/service/register/new/{correlationId}");
        employerAccountService.Verify(x => x.UpsertUserAccount(userId,
            It.Is<UpsertAccountRequest>(c =>
                c.GovIdentifier.Equals(nameClaimValue)
                && c.FirstName.Equals(model.FirstName)
                && c.LastName.Equals(model.LastName)
                && c.CorrelationId.ToString()!.Equals(model.CorrelationId)
                )), Times.Once);
        httpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.GivenName)).Value.Should().Be(model.FirstName);
        httpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.FamilyName)).Value.Should().Be(model.LastName);
        httpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier)).Value.Should().Be(emailClaimValue);
        authenticationService.Verify(x => x.SignInAsync(httpContext, CookieAuthenticationDefaults.AuthenticationScheme, httpContext.User, null));
    }

    [Test, MoqAutoData]
    public async Task When_Valid_Confirmation_Model_And_Auth_Is_Given_AccountService_Called_Claims_Added_And_Return_Redirect_To_RegisterNew_In_Employer_Accounts_When_Invalid_CorrelationId(
        string emailClaimValue,
        string nameClaimValue,
        string userId,
        string firstName,
        string lastName,
        string correlationId,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IEmployerAccountService> employerAccountService,
        [Frozen] Mock<IAuthenticationService> authenticationService,
        [Frozen] Mock<IServiceProvider> serviceProviderMock,
        [Greedy] UserController controller)
    {
        // arrange
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        var model = new AddUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
            CorrelationId = correlationId.ToString()
        };
        serviceProviderMock
            .Setup(_ => _.GetService(typeof(IAuthenticationService)))
            .Returns(authenticationService.Object);

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, emailClaimValue),
                new Claim(ClaimTypes.NameIdentifier, nameClaimValue),
                new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, userId)
            })}),
            RequestServices = serviceProviderMock.Object
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // sut
        var actual = await controller.ConfirmUserDetails(model);

        // assert
        if (actual is RedirectResult result)
            _ = result.Url.Should().BeEquivalentTo($"https://accounts.test-eas.apprenticeships.education.gov.uk/service/register/new/{correlationId}");
        employerAccountService.Verify(x => x.UpsertUserAccount(userId,
            It.Is<UpsertAccountRequest>(c =>
                c.GovIdentifier.Equals(nameClaimValue)
                && c.FirstName.Equals(model.FirstName)
                && c.LastName.Equals(model.LastName)
                && !c.CorrelationId.HasValue
                )), Times.Once);
        httpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.GivenName)).Value.Should().Be(model.FirstName);
        httpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.FamilyName)).Value.Should().Be(model.LastName);
        httpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier)).Value.Should().Be(emailClaimValue);
        authenticationService.Verify(x => x.SignInAsync(httpContext, CookieAuthenticationDefaults.AuthenticationScheme, httpContext.User, null));
    }
}