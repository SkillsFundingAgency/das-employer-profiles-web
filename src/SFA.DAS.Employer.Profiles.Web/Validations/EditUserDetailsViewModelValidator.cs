using FluentValidation;
using SFA.DAS.Employer.Profiles.Web.Models;
using SFA.DAS.InputValidation.Fluent.Extensions;

namespace SFA.DAS.Employer.Profiles.Web.Validations;

public sealed class EditUserDetailsViewModelValidator : AbstractValidator<EditUserDetailsModel>
{
    public EditUserDetailsViewModelValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter your first name")
            .ValidFreeTextCharacters()
            .WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Enter your last name")
            .ValidFreeTextCharacters()
            .WithMessage("Last name contains invalid characters");
    }
}

