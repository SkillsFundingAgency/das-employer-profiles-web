using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.Employer.Profiles.Domain.Employers;
using SFA.DAS.Employer.Profiles.Web.Controllers;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Employer.Profiles.Web.UnitTests.Controllers;

public class ServiceControllerTests
{
    [Test, MoqAutoData]
    public void Then_When_SigningOut_The_View_Is_Returned_With_Model(
        [Frozen] Mock<IConfiguration> configuration, 
        [Greedy] ServiceController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");

        var actual = controller.SignedOut() as ViewResult;

        actual.Should().NotBeNull();
        var actualModel = actual?.Model as SignedOutViewModel;
        Assert.AreEqual("https://accounts.test-eas.apprenticeships.education.gov.uk",actualModel?.ServiceLink);
    }

    [Test, MoqAutoData]
    public void Then_The_Stub_Auth_Is_Created_When_Not_Prod(
        StubAuthUserDetails model,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IStubAuthenticationService> stubAuthService,
        [Greedy] ServiceController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
   
        var httpResponseMock = new Mock<HttpResponse>();
        httpResponseMock.Setup(x => x.Cookies).Returns(new Mock<IResponseCookies>().Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set<IHttpResponseFeature>(new HttpResponseFeature { Body = new MemoryStream() });
        httpContext.Response.OnStarting(() =>
        {
            httpContext.Response.Headers["Header1"] = "Value1";
            return Task.CompletedTask;
        });
        httpContext.Response.OnCompleted(() =>
        {
            httpContext.Response.Headers["Header2"] = "Value2";
            return Task.CompletedTask;
        });
        
        var controllerContext = new ControllerContext { HttpContext = httpContext };
        controller.ControllerContext = controllerContext;

        var actual = controller.AccountDetails(model) as RedirectToRouteResult;

        actual.RouteName.Should().Be(RouteNames.StubSignedIn);
        stubAuthService.Verify(x=>x.AddStubEmployerAuth(It.IsAny<IResponseCookies>(), model), Times.Once);
    }
    
    [Test, MoqAutoData]
    public void Then_The_Stub_Auth_Is_Not_Created_When_Prod(
        StubAuthUserDetails model,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IStubAuthenticationService> stubAuthService,
        [Greedy] ServiceController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("prd");
        
        var actual = controller.AccountDetails(model) as NotFoundResult;

        actual.Should().NotBeNull();
        stubAuthService.Verify(x=>x.AddStubEmployerAuth(It.IsAny<IResponseCookies>(), model), Times.Never);
    }
    
    [Test, MoqAutoData]
    public void Then_The_Stub_Auth_Details_Are_Not_Returned_When_Prod(
        StubAuthUserDetails model,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IStubAuthenticationService> stubAuthService,
        [Greedy] ServiceController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("prd");
        
        var actual = controller.StubSignedIn() as NotFoundResult;

        actual.Should().NotBeNull();
    }
    
    [Test, MoqAutoData]
    public void Then_The_Stub_Auth_Details_Are_Returned_When_Not_Prod(
        string emailClaimValue,
        string nameClaimValue,
        StubAuthUserDetails model,
        EmployerUserAccountItem employerIdentifier,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IStubAuthenticationService> stubAuthService,
        [Greedy] ServiceController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        var httpContext = new DefaultHttpContext();
        var employerAccounts = new Dictionary<string, EmployerUserAccountItem>{{employerIdentifier.AccountId, employerIdentifier}};
        var claim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, JsonConvert.SerializeObject(employerAccounts));
        var emailClaim = new Claim(ClaimTypes.Email, emailClaimValue);
        var nameClaim = new Claim(ClaimTypes.NameIdentifier, nameClaimValue);
        var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            claim,
            emailClaim, 
            nameClaim
        })});
        httpContext.User = claimsPrinciple;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        
        var actual = controller.StubSignedIn() as ViewResult;

        actual.Should().NotBeNull();
        var actualModel = actual.Model as AccountStubViewModel;
        actualModel.Should().NotBeNull();
        actualModel.Email.Should().Be(emailClaimValue);
        actualModel.Id.Should().Be(nameClaimValue);
        actualModel.Accounts.Should().BeEquivalentTo(new List<EmployerUserAccountItem> {employerIdentifier});
    }
    [Test, MoqAutoData]
    public void Then_The_Get_For_Entering_Stub_Auth_Details_Is_Returned_When_Not_Prod(
        StubAuthUserDetails model,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IStubAuthenticationService> stubAuthService,
        [Greedy] ServiceController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
        
        var actual = controller.AccountDetails() as ViewResult;

        actual.Should().NotBeNull();
    }
    
    [Test, MoqAutoData]
    public void Then_The_Get_For_Entering_Stub_Auth_Details_Is_Not_Returned_When_Prod(
        StubAuthUserDetails model,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<IStubAuthenticationService> stubAuthService,
        [Greedy] ServiceController controller)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("prd");
        
        var actual = controller.AccountDetails() as NotFoundResult;

        actual.Should().NotBeNull();
    }
}