using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Employer.Profiles.Web.Models
{
    public class AddUserDetailsModel: ViewModelBase
    {
        private readonly string? _environmentPart;
        private readonly string? _domainPart;

        public AddUserDetailsModel() { }

        public AddUserDetailsModel(string environment)
        {
            _environmentPart = environment.ToLower() == "prd" ? "manage-apprenticeships" : $"{environment.ToLower()}-eas.apprenticeships";
            _domainPart = environment.ToLower() == "prd" ? "service" : "education";
        }

        [Required(ErrorMessage = "Enter your first name")]
        [MinLength(1)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Enter your first name")]
        [MinLength(1)]
        public string LastName { get; set; } = string.Empty;

        public string FirstNameError => GetErrorMessage(nameof(FirstName));
        public string LastNameError => GetErrorMessage(nameof(LastName));
        public string TermsOfUseLink => $"https://accounts.{_environmentPart}.{_domainPart}.gov.uk/service/termsAndConditions/overview";
        public string RedirectUrl => $"https://accounts.{_environmentPart}.{_domainPart}.gov.uk";
    }
}
