using System.Text.Json.Serialization;

namespace SFA.DAS.Employer.Profiles.Domain.OuterApi
{
    public interface IPutApiRequest
    {
        [JsonIgnore]
        string PutUrl { get; }
        object Data { get; set; }
    }
}
