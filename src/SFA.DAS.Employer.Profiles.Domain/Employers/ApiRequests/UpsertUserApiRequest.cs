using System.Web;
using SFA.DAS.Employer.Profiles.Domain.Models;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;

namespace SFA.DAS.Employer.Profiles.Domain.Employers.ApiRequests;

public class UpsertUserApiRequest : IPutApiRequest
{
    private readonly string _userId;

    public UpsertUserApiRequest(string userId, UpsertAccountRequest request)
    {
        _userId = HttpUtility.UrlEncode(userId);
        Data = request;
    }

    public string PutUrl => $"accountusers/{_userId}/upsert-user";
    public object Data { get; set; }
}