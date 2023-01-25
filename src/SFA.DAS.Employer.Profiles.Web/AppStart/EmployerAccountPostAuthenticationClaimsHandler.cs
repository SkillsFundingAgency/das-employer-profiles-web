using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Web.Infrastructure;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Employer.Profiles.Web.AppStart;


public class EmployerAccountPostAuthenticationClaimsHandler : ICustomClaims
{
    private readonly IEmployerAccountService _accountsSvc;

    public EmployerAccountPostAuthenticationClaimsHandler(IEmployerAccountService accountsSvc)
    {
        _accountsSvc = accountsSvc;
    }
    public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext ctx)
    {
        var userId = ctx.Principal.Claims
                .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
                .Value;
        var email = ctx.Principal.Claims
                .First(c => c.Type.Equals(ClaimTypes.Email))
                .Value;
            
        var result = await _accountsSvc.GetUserAccounts(userId, email);

        var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.ToDictionary(k => k.AccountId));
        var associatedAccountsClaim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json);
        return new List<Claim> {associatedAccountsClaim};
    }
}