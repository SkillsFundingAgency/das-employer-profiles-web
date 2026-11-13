using FluentValidation;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.InputValidation.Fluent.Extensions;

namespace SFA.DAS.Employer.Profiles.Web.Validations;

public sealed class AddUserDetailsViewModelValidator : AbstractValidator<AddUserDetailsModel>
{
    public AddUserDetailsViewModelValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter your first name")
            .Must(name => name == null || name.Any(char.IsLetter))
            .WithMessage("Enter a valid first name")
            .ValidFreeTextCharacters().WithMessage("First name must only include letters a to z, numbers 0 to 9, and special characters such as hyphens, spaces and apostrophes");

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter your last name")
            .Must(name => name == null || name.Any(char.IsLetter))
            .WithMessage("Enter a valid last name")
            .Matches(@"^[\p{L}\p{M}\s'-]+$")
            .WithMessage("Last name contains invalid characters");
    }
}

