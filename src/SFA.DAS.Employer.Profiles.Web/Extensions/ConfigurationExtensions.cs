namespace SFA.DAS.Employer.Profiles.Web.Extensions;

public static class ConfigurationExtensions
{
    public static bool IsDev(this IConfiguration configuration)
    {
        return configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }
    public static bool IsLocal(this IConfiguration configuration)
    {
        return configuration["EnvironmentName"].StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase);
    }
}

public static class UrlRedirectionExtensions
{
    public static string GetTermsAndConditionsUrl(string environmentName)
    {
        return $"{GetBaseUrl(environmentName)}service/termsAndConditions/overview";
    }
    public static string GetRedirectUrl(string environmentName)
    {
        return $"{GetBaseUrl(environmentName)}service/index";
    }

    public static string GetProviderRegistrationReturnUrl(string environmentName)
    {
        return $"{GetBaseUrl(environmentName)}service/register/new";
    }

    public static string GetProgressSavedUrl(string environmentName)
    {
        return $"{GetBaseUrl(environmentName)}accounts/create/progress-saved";
    }

    private static string GetBaseUrl(string environment)
    {
        var environmentPart = environment.ToLower() == "prd" ? "manage-apprenticeships" : $"{environment.ToLower()}-eas.apprenticeships";
        var domainPart = environment.ToLower() == "prd" ? "service" : "education";
        return $"https://accounts.{environmentPart}.{domainPart}.gov.uk/";
    }
}