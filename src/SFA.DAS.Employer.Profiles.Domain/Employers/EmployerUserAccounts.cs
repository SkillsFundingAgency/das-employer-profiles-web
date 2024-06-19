using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;

namespace SFA.DAS.Employer.Profiles.Domain.Employers;

public class EmployerUserAccounts
{
    public IEnumerable<EmployerUserAccountItem> EmployerAccounts { get ; set ; }
    public bool IsSuspended { get; set; }
    
    public string EmployerUserId { get; set; }

    public string LastName { get; set; }

    public string FirstName { get; set; }
    public static implicit operator EmployerUserAccounts(GetUserAccountsResponse source)
    {
        var accounts = source?.UserAccounts == null
            ? new List<EmployerUserAccountItem>()
            : source.UserAccounts.Select(c => (EmployerUserAccountItem) c).ToList();
        
        return new EmployerUserAccounts
        {
            EmployerAccounts = accounts,
            IsSuspended = source?.IsSuspended ?? false,
            FirstName = source?.FirstName ?? "",
            LastName = source?.LastName ?? "",
            EmployerUserId = source?.EmployerUserId ?? "",
        };
    }

}

public class EmployerUserAccountItem
{
    public string AccountId { get; set; }
    public string EmployerName { get; set; }
    public string Role { get; set; }
    public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        
    public static implicit operator EmployerUserAccountItem(EmployerIdentifier source)
    {
        return new EmployerUserAccountItem
        {
            AccountId = source.AccountId,
            EmployerName = source.EmployerName,
            Role = source.Role,
            ApprenticeshipEmployerType = source.ApprenticeshipEmployerType
        };
    }
}