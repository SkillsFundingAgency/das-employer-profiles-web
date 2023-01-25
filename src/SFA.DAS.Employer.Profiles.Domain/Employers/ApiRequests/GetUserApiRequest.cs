using System.Web;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;

namespace SFA.DAS.Employer.Profiles.Domain.Employers.ApiRequests;

public class GetUserAccountsRequest : IGetApiRequest
{
    private readonly string _userId;
    private readonly string _email;

    public GetUserAccountsRequest(string userId, string email)
    {
        _userId = userId;
        _email = HttpUtility.UrlEncode(email);
    }

    public string GetUrl => $"accountusers/{_userId}/accounts?email={_email}";
}