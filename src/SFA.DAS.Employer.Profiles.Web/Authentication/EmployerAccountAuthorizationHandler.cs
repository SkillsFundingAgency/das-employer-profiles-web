using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.GovUK.Auth.Employer;

namespace SFA.DAS.Employer.Profiles.Web.Authentication;

public class EmployerAccountAuthorizationHandler(
    IHttpContextAccessor httpContextAccessor,
    IAssociatedAccountsService associatedAccountsService,
    ILogger<EmployerAccountAuthorizationHandler> logger) : AuthorizationHandler<EmployerAccountRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployerAccountRequirement requirement)
    {
        if (!await IsEmployerAuthorised(context))
        {
            return;
        }

        context.Succeed(requirement);
    }

    private async Task<bool> IsEmployerAuthorised(AuthorizationHandlerContext context)
    {
        if (!httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(RouteValueKeys.EncodedAccountId))
        {
            return false;
        }

        var accountIdFromUrl = httpContextAccessor.HttpContext.Request.RouteValues[RouteValueKeys.EncodedAccountId].ToString().ToUpper();
        
        Dictionary<string, EmployerUserAccountItem> employerAccounts;

        try
        {
            employerAccounts = await associatedAccountsService.GetAccounts(forceRefresh: false);
        }
        catch (JsonSerializationException e)
        {
            logger.LogError(e, "Could not deserialize employer account claim for user");
            return false;
        }

        EmployerUserAccountItem employerIdentifier = null;

        if (employerAccounts != null)
        {
            employerIdentifier = employerAccounts.TryGetValue(accountIdFromUrl, out var account)
                ? account
                : null;
        }

        if (employerAccounts == null || !employerAccounts.ContainsKey(accountIdFromUrl))
        {
            if (!context.User.HasClaim(c => c.Type.Equals(ClaimTypes.NameIdentifier)))
            {
                return false;
            }

            var updatedEmployerAccounts = await associatedAccountsService.GetAccounts(forceRefresh: true);

            if (!updatedEmployerAccounts.ContainsKey(accountIdFromUrl))
            {
                return false;
            }

            employerIdentifier = updatedEmployerAccounts[accountIdFromUrl];
        }

        if (!httpContextAccessor.HttpContext.Items.ContainsKey("Employer"))
        {
            httpContextAccessor.HttpContext.Items.Add("Employer", employerAccounts.GetValueOrDefault(accountIdFromUrl));
        }

        return CheckUserRoleForAccess(employerIdentifier);
    }

    private static bool CheckUserRoleForAccess(EmployerUserAccountItem employerIdentifier)
    {
        return Enum.TryParse<EmployerUserRole>(employerIdentifier.Role, true, out _);
    }
}