using System.Security.Claims;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;

namespace SFA.DAS.Employer.Profiles.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(EmployerClaims.IdamsUserIdClaimTypeIdentifier)?.Value;
    }
}