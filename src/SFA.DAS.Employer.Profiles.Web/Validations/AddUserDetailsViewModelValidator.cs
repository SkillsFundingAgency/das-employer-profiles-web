using FluentValidation;
using SFA.DAS.Employer.Profiles.Web.Models;

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
            .Matches(@"^[\p{L}\p{M}][\p{L}\p{M}\s'\-]*$")
            .WithMessage("First name must only include letters, spaces, hyphens and apostrophes");

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter your last name")
            .Must(name => name == null || name.Any(char.IsLetter))
            .WithMessage("Enter a valid last name")
            .Matches(@"^[\p{L}\p{M}][\p{L}\p{M}\s'\-]*$")
            .WithMessage("Last name must only include letters, spaces, hyphens and apostrophes");
    }
}

