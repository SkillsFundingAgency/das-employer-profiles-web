using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Employer.Profiles.Web.Models
{
    public class EditUserDetailsModel : ViewModelBase
    {

        [Required(ErrorMessage = "Enter your first name")]
        [MinLength(1)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your last name")]
        [MinLength(1)]
        public string LastName { get; set; } = string.Empty;

        public string OriginalFirstName { get; set; } = string.Empty;
        public string OriginalLastName { get; set; } = string.Empty;

        public string? CorrelationId { get; set; }

        public string FirstNameError => GetErrorMessage(nameof(FirstName));
        public string LastNameError => GetErrorMessage(nameof(LastName));
        public string HasNoChangeError => GetErrorMessage(nameof(HasNoChange));

        public bool HasNoChange = false;
        public string CancelLink { get; set; } = string.Empty;
    }
}
