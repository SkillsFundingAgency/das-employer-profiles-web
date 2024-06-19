using System.Text.Json.Serialization;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;

public class GetUserAccountsResponse
{
    [JsonPropertyName("isSuspended")]
    public bool IsSuspended { get; set; }
    
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }
    
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }
    [JsonPropertyName("employerUserId")]
    public string EmployerUserId { get; set; }
    
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
    [JsonPropertyName("apprenticeshipEmployerType")]
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
}