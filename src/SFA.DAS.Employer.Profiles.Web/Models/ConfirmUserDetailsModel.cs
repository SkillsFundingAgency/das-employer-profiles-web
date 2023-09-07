namespace SFA.DAS.Employer.Profiles.Web.Models
{
    public class ConfirmUserDetailsModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? CorrelationId { get; set; }
        public string ChangeRoute { get; set; }
    }
}
