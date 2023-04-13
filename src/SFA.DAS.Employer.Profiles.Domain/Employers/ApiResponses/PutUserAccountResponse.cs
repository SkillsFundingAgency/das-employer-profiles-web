using System.Text.Json.Serialization;

namespace SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses
{
    public class PutUserAccountResponse
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }
}
