using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Web.Authentication;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SFA.DAS.Employer.Profiles.Domain.Models;
using SFA.DAS.Employer.Profiles.Web.Extensions;

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
    [HttpGet]
    [Route("[controller]/add-user-details", Name = RouteNames.AddUserDetails)]
    public IActionResult AddUserDetails([FromQuery]string firstName = "", [FromQuery]string lastName = "", [FromQuery]string correlationId = "")
    {
        var addUserDetailsModel = new AddUserDetailsModel
        {
            FirstName = firstName,
            LastName = lastName,
            CorrelationId = correlationId,
            TermsOfUseLink = UrlRedirectionExtensions.GetTermsAndConditionsUrl(_configuration["ResourceEnvironmentName"])
        };
        return View(addUserDetailsModel);
    }
    
    
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpPost]
    [Route("[controller]/add-user-details", Name = RouteNames.AddUserDetails)]
    public async Task<IActionResult> AddUserDetails(AddUserDetailsModel model)
    {
        // check if the model state is valid.
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
                TermsOfUseLink = UrlRedirectionExtensions.GetTermsAndConditionsUrl(_configuration["ResourceEnvironmentName"]),
                CorrelationId = model.CorrelationId,
                FirstName = model.FirstName,
                LastName = model.LastName
            });
        }

        // read the claims from the ClaimsPrincipal.
        var userId = HttpContext.User.Claims.First(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier)).Value;
        var govIdentifier = HttpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
        var email = HttpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.Email)).Value;

        // Add the user details to the repository via Apim.
        _ = await _accountsService.UpsertUserAccount(userId, new UpsertAccountRequest
        {
            FirstName = model.FirstName,
            Email = email,
            LastName = model.LastName,
            GovIdentifier = govIdentifier
        });


        User.Identities.First().AddClaims(new List<Claim>
        {
            new Claim(EmployerClaims.IdamsUserEmailClaimTypeIdentifier, email),
            new Claim(EmployerClaims.GivenName, model.FirstName),
            new Claim(EmployerClaims.FamilyName, model.LastName)
        });

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, User);
        
        // re-direct the user to the default home page of manage apprenticeship service.
        return Redirect(UrlRedirectionExtensions.GetRedirectUrl(_configuration["ResourceEnvironmentName"]));
    }
}