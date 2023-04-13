using SFA.DAS.Employer.Profiles.Domain.Employers;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiRequests;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;
using SFA.DAS.Employer.Profiles.Domain.Models;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;

namespace SFA.DAS.Employer.Profiles.Application.EmployerAccount;

public interface IEmployerAccountService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
    Task<PutUserAccountResponse> UpsertUserAccount(string userId, UpsertAccountRequest request);
}
public class EmployerAccountService : IEmployerAccountService
{
    private readonly IApiClient _apiClient;

    public EmployerAccountService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        var result = await _apiClient.Get<GetUserAccountsResponse>(new GetUserAccountsRequest(userId, email));

        return result.Body;
    }

    public async Task<PutUserAccountResponse> UpsertUserAccount(string userId, UpsertAccountRequest request)
    {
        var result = await _apiClient.Put<PutUserAccountResponse>(new UpsertUserApiRequest(userId, request));

        return result.Body;
    }
}