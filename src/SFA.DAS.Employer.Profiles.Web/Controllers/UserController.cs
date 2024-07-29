using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Domain.Models;
using SFA.DAS.Employer.Profiles.Web.Authentication;
using SFA.DAS.Employer.Profiles.Web.Extensions;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;

namespace SFA.DAS.Employer.Profiles.Web.Controllers;



public class UserController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IEmployerAccountService _accountsService;

    public UserController(IConfiguration configuration, IEmployerAccountService accountsService)
    {
        _configuration = configuration;
        _accountsService = accountsService;
    }

    [SetNavigationSection(NavigationSection.AccountsHome)]
    [Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
    [HttpGet]
    [Route("accounts/{employerAccountId}/[controller]/change-sign-in-details", Name = RouteNames.ChangeSignInDetails)]
    public IActionResult ChangeSignInDetails()
    {
        var model = new ChangeSignInDetailsViewModel(_configuration["ResourceEnvironmentName"]);
        return View(model);
    }

    [SetNavigationSection(NavigationSection.None)]
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [Route("accounts/[controller]/change-sign-in-details", Name = RouteNames.ChangeSignInDetailsNoAccount)]
    public IActionResult ChangeSignInDetailsNoAccount()
    {
        var model = new ChangeSignInDetailsViewModel(_configuration["ResourceEnvironmentName"]);
        return View("ChangeSignInDetails", model);
    }

    [SetNavigationSection(NavigationSection.None)]
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpGet]
    [Route("[controller]/add-user-details", Name = RouteNames.AddUserDetails)]
    public IActionResult AddUserDetails([FromQuery] string firstName = "", [FromQuery] string lastName = "", [FromQuery] string correlationId = "")
    {
        var addUserDetailsModel = new AddUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
            CorrelationId = correlationId,
            TermsOfUseLink = UrlRedirectionExtensions.GetTermsAndConditionsUrl(_configuration["ResourceEnvironmentName"])
        };
        ModelState.Clear();
        return View(addUserDetailsModel);
    }


    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpPost]
    [Route("[controller]/add-user-details", Name = RouteNames.AddUserDetails)]
    public IActionResult AddUserDetails(AddUserDetailsModel model)
    {
        // check if the model state is valid.
        if (!ModelState.IsValid)
        {
            return View(new AddUserDetailsModel
            {
                ErrorDictionary = ModelState
                    .Where(x => x.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                    ),
                TermsOfUseLink = UrlRedirectionExtensions.GetTermsAndConditionsUrl(_configuration["ResourceEnvironmentName"]),
                CorrelationId = model.CorrelationId,
                FirstName = model.FirstName,
                LastName = model.LastName
            });
        }

        return RedirectToRoute(RouteNames.ConfirmUserDetails, new { model.FirstName, model.LastName, model.CorrelationId });
    }

    [SetNavigationSection(NavigationSection.None)]
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpGet]
    [Route("[controller]/edit-user-details", Name = RouteNames.EditUserDetails)]
    public IActionResult EditUserDetails([FromQuery] string firstName = "", [FromQuery] string lastName = "", [FromQuery] string correlationId = "")
    {
        var editUserDetailsModel = new EditUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
            OriginalFirstName = firstName,
            OriginalLastName = lastName,
            CorrelationId = correlationId,
            CancelLink = $"{UrlRedirectionExtensions.GetRedirectUrl(_configuration["ResourceEnvironmentName"])}"
        };
        ModelState.Clear();
        return View(editUserDetailsModel);
    }

    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpPost]
    [Route("[controller]/edit-user-details", Name = RouteNames.EditUserDetails)]
    public IActionResult EditUserDetails(EditUserDetailsModel model)
    {
        var noChangeHasBeenMade = model.FirstName == model.OriginalFirstName && model.LastName == model.OriginalLastName;
        if (noChangeHasBeenMade)
        {
            ModelState.AddModelError(nameof(model.HasNoChange), "Make changes to user details or select 'Cancel'");
        }

        // check if the model state is valid.
        if (!ModelState.IsValid)
        {
            return View(new EditUserDetailsModel
            {
                ErrorDictionary = ModelState
                    .Where(x => x.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                    ),
                CancelLink = $"{UrlRedirectionExtensions.GetRedirectUrl(_configuration["ResourceEnvironmentName"])}",
                CorrelationId = model.CorrelationId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                OriginalFirstName = model.OriginalFirstName,
                OriginalLastName = model.OriginalLastName,
                HasNoChange = noChangeHasBeenMade
            });
        }

        return RedirectToRoute(RouteNames.ConfirmUserDetails, new { model.FirstName, model.LastName, model.CorrelationId, isEdit = true });
    }

    [SetNavigationSection(NavigationSection.None)]
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpGet]
    [Route("[controller]/confirm-user-details", Name = RouteNames.ConfirmUserDetails)]
    public IActionResult ConfirmUserDetails(
        [FromQuery] string firstName = "",
        [FromQuery] string lastName = "",
        [FromQuery] string correlationId = "",
        [FromQuery] bool isEdit = false)
    {
        var confirmUserDetailsModel = new ConfirmUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
            CorrelationId = correlationId,
            ChangeRoute = isEdit ? RouteNames.EditUserDetails : RouteNames.AddUserDetails,
            IsEdit = isEdit
        };

        ModelState.Clear();
        return View(confirmUserDetailsModel);
    }

    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpPost]
    [Route("[controller]/confirm-user-details", Name = RouteNames.ConfirmUserDetails)]
    public async Task<IActionResult> ConfirmUserDetails(AddUserDetailsModel model)
    {
        // read the claims from the ClaimsPrincipal.
        var userId = HttpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Value;
        var govIdentifier = HttpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
        var email = HttpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.Email)).Value;

        // Add the user details to the repository via Apim.
        Guid.TryParse(model.CorrelationId, out var correlationId);
        _ = await _accountsService.UpsertUserAccount(userId, new UpsertAccountRequest
        {
            FirstName = model.FirstName,
            Email = email,
            LastName = model.LastName,
            GovIdentifier = govIdentifier,
            CorrelationId = correlationId == Guid.Empty ? null : correlationId
        });


        User.Identities.First().AddClaims(new List<Claim>
        {
            new Claim(EmployerClaims.IdamsUserEmailClaimTypeIdentifier, email),
            new Claim(EmployerClaims.GivenName, model.FirstName),
            new Claim(EmployerClaims.FamilyName, model.LastName)
        });

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, User);

        return RedirectToRoute(RouteNames.UserDetailsSuccess, new { model.IsEdit });
    }

    [SetNavigationSection(NavigationSection.None)]
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpGet]
    [Route("[controller]/user-details-success", Name = RouteNames.UserDetailsSuccess)]
    public IActionResult UserDetailsSuccess([FromQuery] string correlationId = "", [FromQuery] bool isEdit = false)
    {
        var returnUrl = string.IsNullOrEmpty(correlationId)
            ? $"{UrlRedirectionExtensions.GetRedirectUrl(_configuration["ResourceEnvironmentName"])}"
            : $"{UrlRedirectionExtensions.GetProviderRegistrationReturnUrl(_configuration["ResourceEnvironmentName"])}/{correlationId}";

        return View(new UserDetailsSuccessModel
        {
            CorrelationId = correlationId,
            AccountReturnUrl = returnUrl,
            AccountSaveAndComeBackLaterUrl = UrlRedirectionExtensions.GetProgressSavedUrl(_configuration["ResourceEnvironmentName"]),
            IsEdit = isEdit
        });
    }
}