using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Employer.Profiles.Web.Authentication;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;

namespace SFA.DAS.Employer.Profiles.Web.Controllers;

[Route("[controller]")]
[SetNavigationSection(NavigationSection.AccountsFinance)]
[Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
public class EmployerController : Controller
{
    [HttpGet]
    [Route("change-sign-in-details", Name = RouteNames.ChangeSignInDetails)]
    public async Task<IActionResult> ChangeSignInDetails()
    {
       return View();
    }
}