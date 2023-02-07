using System.Text.Json.Serialization;

namespace SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;

public class GetUserAccountsResponse
{
    [JsonPropertyName("userAccounts")]
    public List<EmployerIdentifier> UserAccounts { get; set; }
}
    
public class EmployerIdentifier
{
    [JsonPropertyName("encodedAccountId")]
    public string AccountId { get; set; }
    [JsonPropertyName("dasAccountName")]
    public string EmployerName { get; set; }
    [JsonPropertyName("role")]
    public string Role { get; set; }
    [JsonPropertyName("IsSuspended")]
    public bool IsSuspended { get; set; }
}