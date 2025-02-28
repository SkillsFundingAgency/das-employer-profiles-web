using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Employer.Profiles.Web.Controllers;

public class CookieConsentController : Controller
{
    [HttpGet]
    [Route("cookieConsent")]        
    public IActionResult Settings()
    {
        return View();
    }

    [HttpGet]
    [Route("cookieConsent/details")]
    public IActionResult Details()
    {
        return View();
    }
}