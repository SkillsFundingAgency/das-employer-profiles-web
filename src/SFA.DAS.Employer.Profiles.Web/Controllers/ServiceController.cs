using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;

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
        return View("SignedOut",@"https://localhost:/somewhere");
    }
}