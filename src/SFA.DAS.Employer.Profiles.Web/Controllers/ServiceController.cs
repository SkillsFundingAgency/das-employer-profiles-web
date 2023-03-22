using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Employer.Profiles.Domain.Employers;
using SFA.DAS.Employer.Profiles.Web.Authentication;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.Employer.Profiles.Web.Controllers;

[Route("[controller]")]
public class ServiceController : Controller
{
    private readonly IConfiguration _configuration;

    public ServiceController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    [Route("signout", Name = RouteNames.SignOut)]
    public async Task<IActionResult> SignOut()
    {
        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.Parameters.Clear();
        authenticationProperties.Parameters.Add("id_token",idToken);
        return SignOut(
            authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    [Route("user-signed-out", Name = RouteNames.SignedOut)]
    [HttpGet]
    public IActionResult SignedOut()
    {
        return View("SignedOut", new SignedOutViewModel(_configuration["ResourceEnvironmentName"]));
    }

    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [Route("account-unavailable", Name = RouteNames.AccountUnavailable)]
    public IActionResult AccountUnavailable()
    {
        return View();
    }
    
    [HttpGet]
    [Route("account-details", Name = RouteNames.StubAccountDetailsGet)]
    public IActionResult AccountDetails()
    {
        return View();
    }
    [HttpPost]
    [Route("account-details", Name = RouteNames.StubAccountDetailsPost)]
    public IActionResult AccountDetails(StubAuthUserDetails model)
    {
        var authCookie = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddMinutes(2),
            Path = "/",
            Domain = ".test-eas.apprenticeships.education.gov.uk"
        };
        Response.Cookies.Append(GovUkConstants.StubAuthCookieName,JsonConvert.SerializeObject(model),authCookie);
        
        var authCookie2 = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddMinutes(2),
            Path = "/",
            Domain = "localhost"
        };
        Response.Cookies.Append($"{GovUkConstants.StubAuthCookieName}",JsonConvert.SerializeObject(model),authCookie2);
        
        return RedirectToRoute(RouteNames.StubSignedIn);
    }

    [HttpGet]
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [Route("Stub-Auth", Name = RouteNames.StubSignedIn)]
    public IActionResult StubSignedIn() 
    {
        var viewModel = new AccountStubViewModel
        {
            Email = User.Claims.FirstOrDefault(c=>c.Type.Equals(ClaimTypes.Email))?.Value,
            Id = User.Claims.FirstOrDefault(c=>c.Type.Equals(ClaimTypes.NameIdentifier))?.Value,
            Accounts = JsonConvert.DeserializeObject<Dictionary<string,EmployerUserAccountItem>>(
                User.Claims.FirstOrDefault(c=>c.Type.Equals(EmployerClaims.AccountsClaimsTypeIdentifier))?.Value)
                .Select(c=>c.Value)
                .ToList()

        };
        return View(viewModel);
    }
}