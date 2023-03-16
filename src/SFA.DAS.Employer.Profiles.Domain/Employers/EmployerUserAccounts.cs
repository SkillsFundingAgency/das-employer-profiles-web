using SFA.DAS.Employer.Profiles.Domain.Employers.ApiResponses;

namespace SFA.DAS.Employer.Profiles.Domain.Employers;

public class EmployerUserAccounts
{
    public IEnumerable<EmployerUserAccountItem> EmployerAccounts { get ; set ; }
    public bool IsSuspended { get; set; }
    public static implicit operator EmployerUserAccounts(GetUserAccountsResponse source)
    {
        var accounts = source?.UserAccounts == null
            ? new List<EmployerUserAccountItem>()
            : source.UserAccounts.Select(c => (EmployerUserAccountItem) c).ToList();
        
        return new EmployerUserAccounts
        {
            EmployerAccounts = accounts,
            IsSuspended = source?.IsSuspended ?? false,
        };
    }
}

public class EmployerUserAccountItem
{
    public string AccountId { get; set; }
    public string EmployerName { get; set; }
    public string Role { get; set; }
        
    public static implicit operator EmployerUserAccountItem(EmployerIdentifier source)
    {
        return new EmployerUserAccountItem
        {
            AccountId = source.AccountId,
            EmployerName = source.EmployerName,
            Role = source.Role
        };
    }
}