using SFA.DAS.Employer.Profiles.Domain.Employers.ApiRequests;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;
using SFA.DAS.Employer.Profiles.Domain.Models;
using SFA.DAS.Employer.Profiles.Domain.OuterApi;
using SFA.DAS.GovUK.Auth.Employer;

namespace SFA.DAS.Employer.Profiles.Application.EmployerAccount;

public interface IEmployerAccountService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
    Task<PutUserAccountResponse> UpsertUserAccount(string userId, UpsertAccountRequest request);
}

public class EmployerAccountService(IApiClient apiClient) : IEmployerAccountService, IGovAuthEmployerAccountService
{
    public async Task<EmployerUserAccounts> GetUserAccounts(string userId, string email)
    {
        var actual = await apiClient.Get<GetUserAccountsResponse>(new GetUserAccountsRequest(userId, email));

        var result = actual.Body;
        
        return new EmployerUserAccounts
        {
            EmployerAccounts = result.UserAccounts != null? result.UserAccounts.Select(c => new EmployerUserAccountItem
            {
                Role = c.Role,
                AccountId = c.AccountId,
                ApprenticeshipEmployerType = Enum.Parse<ApprenticeshipEmployerType>(c.ApprenticeshipEmployerType.ToString()),
                EmployerName = c.EmployerName,
            }).ToList() : [],
            FirstName = result.FirstName,
            IsSuspended = result.IsSuspended,
            LastName = result.LastName,
            EmployerUserId = result.EmployerUserId,
        };
    }

    public async Task<PutUserAccountResponse> UpsertUserAccount(string userId, UpsertAccountRequest request)
    {
        var result = await apiClient.Put<PutUserAccountResponse>(new UpsertUserApiRequest(userId, request));

        return result.Body;
    }
}