using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Employer.Profiles.Web.Models
{
    public class AddUserDetailsModel : ViewModelBase
    {
        public string FirstName { get; set; } = string.Empty;        
        public string LastName { get; set; } = string.Empty;
        public string? CorrelationId { get; set; }        
        public string FirstNameError => GetErrorMessage(nameof(FirstName));
        public string LastNameError => GetErrorMessage(nameof(LastName));
        public string TermsOfUseLink { get; set; } = string.Empty;
        public bool IsEdit { get; set; } = false;
    }
}
