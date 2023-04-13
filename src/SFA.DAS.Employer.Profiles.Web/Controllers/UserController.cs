using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Employer.Profiles.Web.Authentication;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;

namespace SFA.DAS.Employer.Profiles.Web.Controllers;

[Route("accounts/{employerAccountId}/[controller]")]
[SetNavigationSection(NavigationSection.AccountsHome)]
[Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
public class UserController : Controller
{
    private readonly IConfiguration _configuration;

    public UserController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    [Route("change-sign-in-details", Name = RouteNames.ChangeSignInDetails)]
    public IActionResult ChangeSignInDetails()
    {
        var model = new ChangeSignInDetailsViewModel(_configuration["ResourceEnvironmentName"]);
        return View(model);
    }

    [HttpGet]
    [Route("add-user-details", Name = RouteNames.AddUserDetails)]
    public IActionResult AddUserDetails()
    {
        return View(new AddUserDetailsModel());
    }

    [HttpPost]
    [Route("add-user-details", Name = RouteNames.AddUserDetails)]
    public IActionResult AddUserDetails(AddUserDetailsModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(new AddUserDetailsModel
            {
                ErrorDictionary = ModelState
                    .Where(x => x.Value is {Errors.Count: > 0})
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                    ),
                FirstName = model.FirstName,
                LastName = model.LastName
        });
        }

        return View(model);
    }
}