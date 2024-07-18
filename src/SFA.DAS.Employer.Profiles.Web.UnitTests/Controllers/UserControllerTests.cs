using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Domain.Models;
using SFA.DAS.Employer.Profiles.Web.Controllers;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.Testing.AutoFixture;

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
        actualModel?.SettingsLink.Should().BeEquivalentTo("https://home.integration.account.gov.uk/settings");
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
        actualModel?.SettingsLink.Should().BeEquivalentTo("https://home.integration.account.gov.uk/settings");
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

        actual.Should().NotBeNull();
        var actualViewResult = actual as ViewResult;
        actualViewResult.Should().NotBeNull();
        var actualModel = actualViewResult!.Model as AddUserDetailsModel;
        actualModel.Should().NotBeNull();
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

        actual.Should().NotBeNull();
        var actualViewResult = actual as ViewResult;
        actualViewResult.Should().NotBeNull();
        var actualModel = actualViewResult!.Model as AddUserDetailsModel;
        actualModel.Should().NotBeNull();
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
    public void When_Edit_Then_The_FirstName_LastName_And_CorrelationId_Are_Passed_To_The_View(
        string firstName,
        string lastName,
        string correlationId,
        [Frozen] Mock<IConfiguration> configuration,
        [Greedy] UserController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("prd");

        var actual = controller.EditUserDetails(firstName, lastName, correlationId);

        actual.Should().NotBeNull();
        var actualViewResult = actual as ViewResult;
        actualViewResult.Should().NotBeNull();
        var actualModel = actualViewResult!.Model as EditUserDetailsModel;
        actualModel.Should().NotBeNull();
        actualModel!.FirstName.Should().Be(firstName);
        actualModel!.OriginalFirstName.Should().Be(firstName);
        actualModel!.LastName.Should().Be(lastName);
        actualModel!.OriginalLastName.Should().Be(lastName);
        actualModel!.CorrelationId.Should().Be(correlationId);
        actualModel.CancelLink.Should().Be("https://accounts.manage-apprenticeships.service.gov.uk/service/index");
        actualModel.HasNoChange.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public void When_Edit_Invalid_Model_And_Auth_Is_Given_Throw_Invalid_ModelState(
        string emailClaimValue,
        string nameClaimValue,
        string errorMessage,
        EditUserDetailsModel model,
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

        model.OriginalFirstName = model.FirstName;
        model.OriginalLastName = model.LastName;

        // sut
        var actual = (ViewResult)controller.EditUserDetails(model);

        // assert
        actual.Should().NotBeNull();
        var actualModel = actual.Model as EditUserDetailsModel;
        actualModel.FirstNameError.Length.Should().BeGreaterThanOrEqualTo(1);
        actualModel.LastNameError.Length.Should().BeGreaterThanOrEqualTo(1);
        actualModel.HasNoChange.Should().BeTrue();
        actualModel.HasNoChangeError.Length.Should().BeGreaterThanOrEqualTo(1);
    }

    [Test, MoqAutoData]
    public void When_Edit_Valid_Model_And_Auth_Is_Given_AccountService_Called_Claims_Added_And_Return_Redirect_Confirm(
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
        var model = new EditUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
            OriginalFirstName = "originalFirst",
            OriginalLastName = "originalLast"
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
        var actual = controller.EditUserDetails(model) as RedirectToRouteResult;

        // assert
        actual.RouteName.Should().Be(RouteNames.ConfirmUserDetails);
    }

    [Test, MoqAutoData]
    public void Then_The_FirstName_LastName_And_CorrelationId_And_IsEdit_Are_Passed_To_The_Confirm_View(
        string firstName,
        string lastName,
        string correlationId,
        bool isEdit,
        [Frozen] Mock<IConfiguration> configuration,
        [Greedy] UserController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("prd");

        var actual = controller.ConfirmUserDetails(firstName, lastName, correlationId, isEdit);

        actual.Should().NotBeNull();
        var actualViewResult = actual as ViewResult;
        actualViewResult.Should().NotBeNull();
        var actualModel = actualViewResult!.Model as ConfirmUserDetailsModel;
        actualModel.Should().NotBeNull();
        actualModel!.FirstName.Should().Be(firstName);
        actualModel!.LastName.Should().Be(lastName);
        actualModel!.CorrelationId.Should().Be(correlationId);
        actualModel!.IsEdit.Should().Be(isEdit);

    }

    [Test, MoqAutoData]
    public async Task When_Valid_Confirmation_Model_And_Auth_Is_Given_AccountService_Called_Claims_Added_And_Return_Redirect_To_Success(
        string emailClaimValue,
        string nameClaimValue,
        string userId,
        string firstName,
        string lastName,
        Guid correlationId,
        [Frozen] IUrlHelper urlHelper,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IEmployerAccountService> employerAccountService,
        [Frozen] Mock<IAuthenticationService> authenticationService,
        [Frozen] Mock<IServiceProvider> serviceProviderMock,
        [Greedy] UserController controller)
    {
        // arrange
        controller.Url = urlHelper;
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
        if (actual is RedirectToRouteResult result)
            _ = result.RouteName.Should().Be(RouteNames.UserDetailsSuccess);
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



    [Test]
    [MoqInlineAutoData(null, true, "https://accounts.manage-apprenticeships.service.gov.uk/service/index")]
    [MoqInlineAutoData("", false, "https://accounts.manage-apprenticeships.service.gov.uk/service/index")]
    [MoqInlineAutoData("correlationId-ab926815-0573-4ea9-89d1-28a6bab2f338", null, "https://accounts.manage-apprenticeships.service.gov.uk/service/register/new/correlationId-ab926815-0573-4ea9-89d1-28a6bab2f338")]
    public void Then_Account_Success_Continue_Urls_Are_Set(
        string correlationId,
        bool isEdit,
        string expectedReturnUrl,
        [Frozen] Mock<IConfiguration> configuration,
        [Greedy] UserController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("prd");

        var actual = controller.UserDetailsSuccess(correlationId, isEdit);

        actual.Should().NotBeNull();
        var actualViewResult = actual as ViewResult;
        actualViewResult.Should().NotBeNull();
        var actualModel = actualViewResult!.Model as UserDetailsSuccessModel;
        actualModel.Should().NotBeNull();
        actualModel!.CorrelationId.Should().Be(correlationId);
        actualModel.IsEdit.Should().Be(isEdit);
        actualModel.AccountSaveAndComeBackLaterUrl.Should().Be("https://accounts.manage-apprenticeships.service.gov.uk/accounts/create/progress-saved");
        actualModel.AccountReturnUrl.Should().Be(expectedReturnUrl);
    }
}