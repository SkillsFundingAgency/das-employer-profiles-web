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

public class EmployerAccountService(IApiClient apiClient) : IEmployerAccountService
{
    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        var result = await apiClient.Get<GetUserAccountsResponse>(new GetUserAccountsRequest(userId, email));

        return result.Body;
    }

    public async Task<PutUserAccountResponse> UpsertUserAccount(string userId, UpsertAccountRequest request)
    {
        var result = await apiClient.Put<PutUserAccountResponse>(new UpsertUserApiRequest(userId, request));

        return result.Body;
    }
}