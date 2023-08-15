namespace SFA.DAS.Employer.Profiles.Web.Models
{
    public class ConfirmUserDetailsModel : ViewModelBase
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? CorrelationId { get; set; }
    }
}
