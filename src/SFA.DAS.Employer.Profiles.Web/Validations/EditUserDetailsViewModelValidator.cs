using FluentValidation;
using SFA.DAS.Employer.Profiles.Web.Extensions;
using SFA.DAS.Employer.Profiles.Web.Models;

namespace SFA.DAS.Employer.Profiles.Web.Validations;

public sealed class EditUserDetailsViewModelValidator : AbstractValidator<EditUserDetailsModel>
{
    public EditUserDetailsViewModelValidator()
    {
        NameValidationExtensions.AddNameRules(
            this, x => x.FirstName, "First name");

        NameValidationExtensions.AddNameRules(
            this, x => x.LastName, "Last name");
    }
}

