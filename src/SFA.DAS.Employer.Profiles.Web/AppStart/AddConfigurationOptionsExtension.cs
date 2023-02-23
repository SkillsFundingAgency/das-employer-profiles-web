using Microsoft.Extensions.Options;
using SFA.DAS.Employer.Profiles.Domain.Configuration;

namespace SFA.DAS.Employer.Profiles.Web.AppStart;

public static class AddConfigurationOptionsExtension
{
    public static void AddConfigurationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmployerProfilesWebConfiguration>(configuration.GetSection(nameof(EmployerProfilesWebConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerProfilesWebConfiguration>>().Value);
    }
}