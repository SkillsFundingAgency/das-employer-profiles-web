using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.Employer.Profiles.Web.Models;

public class StubAuthenticationViewModel : StubAuthUserDetails
{
    public string ReturnUrl { get; set; }
}