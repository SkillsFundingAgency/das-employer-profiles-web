using SFA.DAS.Employer.Profiles.Application.EmployerAccount;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;
using SFA.DAS.Employer.Profiles.Infrastructure.Api;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.Employer.Profiles.Web.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services)
    {
        services.AddTransient<IEmployerAccountService, EmployerAccountService>();
        services.AddTransient<ICustomClaims, EmployerAccountPostAuthenticationClaimsHandler>();
        services.AddHttpClient<IApiClient, ApiClient>();
    }
}