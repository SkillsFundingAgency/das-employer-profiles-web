namespace SFA.DAS.Employer.Profiles.Web.Models
{
    public class UserDetailsSuccessModel
    {
        public string AccountReturnUrl { get; set; }
        public string AccountSaveAndComeBackLaterUrl { get; set; }
        public string? CorrelationId { get; set; }
    }
}
