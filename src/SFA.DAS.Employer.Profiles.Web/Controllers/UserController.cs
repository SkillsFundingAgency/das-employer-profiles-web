using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Web.Authentication;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using System.Security.Claims;
using SFA.DAS.Employer.Profiles.Domain.Models;

namespace SFA.DAS.Employer.Profiles.Web.Controllers;

[Route("accounts/{employerAccountId}/[controller]")]
[SetNavigationSection(NavigationSection.AccountsHome)]
//[Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
public class UserController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IEmployerAccountService _accountsService;

    public UserController(IConfiguration configuration, IEmployerAccountService accountsService)
    {
        _configuration = configuration;
        _accountsService = accountsService;
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
        return View(new AddUserDetailsModel(_configuration["ResourceEnvironmentName"]));
    }

    [HttpPost]
    [Route("add-user-details", Name = RouteNames.AddUserDetails)]
    public async Task<IActionResult> AddUserDetails(AddUserDetailsModel model)
    {
        // check if the model state is valid.
        if (!ModelState.IsValid)
        {
            return View(new AddUserDetailsModel(_configuration["ResourceEnvironmentName"])
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

        // read the claims from the ClaimsPrincipal.
        var userId = HttpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
        var email = HttpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.Email)).Value;

        // Add the user details to the repository via Apim.
        _ = await _accountsService.UpsertUserAccount(userId, new UpsertAccountRequest
        {
            FirstName = model.FirstName,
            Email = email,
            LastName = model.LastName,
            GovIdentifier = userId
        });

        // re-direct the user to the Add Paye Scheme page.
        return RedirectToRoute(RouteNames.AddPayeScheme);
    }
}