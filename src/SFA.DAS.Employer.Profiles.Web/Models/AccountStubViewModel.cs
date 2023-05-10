using SFA.DAS.Employer.Profiles.Domain.Employers;

namespace SFA.DAS.Employer.Profiles.Web.Models;

public class AccountStubViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public List<EmployerUserAccountItem> Accounts { get; set; }
    public string ReturnUrl { get; set; }
}