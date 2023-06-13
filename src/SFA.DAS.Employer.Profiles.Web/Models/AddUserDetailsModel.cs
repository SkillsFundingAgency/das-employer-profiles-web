using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Employer.Profiles.Web.Models
{
    public class AddUserDetailsModel : ViewModelBase
    {

        [Required(ErrorMessage = "Enter your first name")]
        [MinLength(1)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your last name")]
        [MinLength(1)]
        public string LastName { get; set; } = string.Empty;

        public string FirstNameError => GetErrorMessage(nameof(FirstName));
        public string LastNameError => GetErrorMessage(nameof(LastName));
        public string TermsOfUseLink { get; set; } = string.Empty;
    }
}
