using System.Text.Json.Serialization;

namespace SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;

public class GetUserAccountsResponse
{
    [JsonPropertyName("UserAccounts")]
    public List<EmployerIdentifier> UserAccounts { get; set; }
}
    
public class EmployerIdentifier
{
    [JsonPropertyName("EncodedAccountId")]
    public string AccountId { get; set; }
    [JsonPropertyName("DasAccountName")]
    public string EmployerName { get; set; }
    [JsonPropertyName("Role")]
    public string Role { get; set; }
}