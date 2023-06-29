using Microsoft.AspNetCore.DataProtection;
using SFA.DAS.Employer.Profiles.Domain.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.Employer.Profiles.Web.AppStart;

public static class AddDataProtectionExtensions
{
    public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
            
        var config = configuration.GetSection(nameof(EmployerProfilesWebConfiguration))
            .Get<EmployerProfilesWebConfiguration>();

        if (config != null 
            && !string.IsNullOrEmpty(config.DataProtectionKeysDatabase) 
            && !string.IsNullOrEmpty(config.RedisConnectionString))
        {
            var redisConnectionString = config.RedisConnectionString;
            var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

            var configurationOptions = ConfigurationOptions.Parse($"{redisConnectionString},{dataProtectionKeysDatabase}");
            configurationOptions.SocketManager = SocketManager.ThreadPool;
            var redis = ConnectionMultiplexer
                .Connect(configurationOptions);

            services.AddDataProtection()
                .SetApplicationName("das-employer")
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
        }
    }
}