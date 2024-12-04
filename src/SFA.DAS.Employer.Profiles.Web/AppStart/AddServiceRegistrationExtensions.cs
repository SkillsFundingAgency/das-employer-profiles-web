using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;
using SFA.DAS.Employer.Profiles.Infrastructure.Api;
using SFA.DAS.Employer.Profiles.Web.Authentication;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Employer;
using EmployerClaims = SFA.DAS.Employer.Profiles.Web.Infrastructure.EmployerClaims;

namespace SFA.DAS.Employer.Profiles.Web.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services)
    {
        services.AddTransient<IEmployerAccountService, EmployerAccountService>();
        services.AddTransient<IGovAuthEmployerAccountService, EmployerAccountService>();
        services.AddTransient<IAssociatedAccountsService, AssociatedAccountsService>();
        services.AddHttpClient<IApiClient, ApiClient>();
    }

    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IAuthorizationHandler, EmployerAccountAuthorizationHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyNames.IsAuthenticated, policy => policy.RequireAuthenticatedUser())
            .AddPolicy(PolicyNames.HasEmployerAccount, policy =>
            {
                policy.RequireClaim(EmployerClaims.AccountsClaimsTypeIdentifier);
                policy.Requirements.Add(new EmployerAccountRequirement());
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new AccountActiveRequirement());
            });
    }
}