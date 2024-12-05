using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Domain.Employers;
using SFA.DAS.Employer.Profiles.Web.Authentication;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.GovUK.Auth.Services;
using EmployerClaims = SFA.DAS.Employer.Profiles.Web.Infrastructure.EmployerClaims;

namespace SFA.DAS.Employer.Profiles.Web.Controllers;

[Route("[controller]")]
public class ServiceController(
    IConfiguration configuration,
    IStubAuthenticationService stubAuthenticationService,
    IAssociatedAccountsService associatedAccountsService,
    ILogger<ServiceController> logger)
    : Controller
{
    [Route("signout", Name = RouteNames.SignOut)]
    public async Task<IActionResult> SignOut()
    {
        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.Parameters.Clear();
        authenticationProperties.Parameters.Add("id_token", idToken);

        var schemes = new List<string>
        {
            CookieAuthenticationDefaults.AuthenticationScheme
        };

        _ = bool.TryParse(configuration["StubAuth"], out var stubAuth);

        if (!stubAuth)
        {
            schemes.Add(OpenIdConnectDefaults.AuthenticationScheme);
        }

        return SignOut(
            authenticationProperties,
            schemes.ToArray());
    }

    [AllowAnonymous]
    [Route("user-signed-out", Name = RouteNames.SignedOut)]
    [HttpGet]
    public IActionResult SignedOut()
    {
        return View("SignedOut", new SignedOutViewModel(configuration["ResourceEnvironmentName"]));
    }

    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [Route("account-unavailable", Name = RouteNames.AccountUnavailable)]
    public IActionResult AccountUnavailable()
    {
        return View();
    }

    [HttpGet]
    [Route("account-details", Name = RouteNames.StubAccountDetailsGet)]
    public IActionResult AccountDetails([FromQuery] string returnUrl)
    {
        if (configuration["ResourceEnvironmentName"].ToUpper() == "PRD")
        {
            return NotFound();
        }

        return View("AccountDetails", new StubAuthenticationViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [Route("account-details", Name = RouteNames.StubAccountDetailsPost)]
    public async Task<IActionResult> AccountDetails(StubAuthenticationViewModel model)
    {
        if (configuration["ResourceEnvironmentName"].ToUpper() == "PRD")
        {
            return NotFound();
        }

        logger.LogWarning("ServiceController. StubAuthenticationViewModel: {Model}", JsonConvert.SerializeObject(model));

        var claims = await stubAuthenticationService.GetStubSignInClaims(model);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claims,
            new AuthenticationProperties()
        );

        return RedirectToRoute(RouteNames.StubSignedIn, new { returnUrl = model.ReturnUrl });
    }

    [HttpGet]
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [Route("Stub-Auth", Name = RouteNames.StubSignedIn)]
    public async Task<IActionResult> StubSignedIn([FromQuery] string returnUrl)
    {
        if (configuration["ResourceEnvironmentName"].ToUpper() == "PRD")
        {
            return NotFound();
        }

        var associatedAccounts = await associatedAccountsService.GetAccounts(forceRefresh: false);

        var viewModel = new AccountStubViewModel
        {
            Email = User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value,
            Id = User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))?.Value,
            Accounts = associatedAccounts.Select(c => c.Value).ToList(),
            ReturnUrl = returnUrl
        };
        
        return View(viewModel);
    }
}