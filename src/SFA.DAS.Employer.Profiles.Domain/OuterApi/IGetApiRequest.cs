using System.Text.Json.Serialization;

namespace SFA.DAS.Employer.Profiles.Domain.OuterApi;

public interface IGetApiRequest
{
    [JsonIgnore]
    string GetUrl { get; }
}