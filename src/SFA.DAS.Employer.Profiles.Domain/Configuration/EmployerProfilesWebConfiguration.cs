namespace SFA.DAS.Employer.Profiles.Domain.Configuration;

public class EmployerProfilesWebConfiguration
{
    public string DataProtectionKeysDatabase { get; set; }
    public string RedisConnectionString { get; set; }
    public string Key { get; set; }
    public string BaseUrl { get; set; }
}