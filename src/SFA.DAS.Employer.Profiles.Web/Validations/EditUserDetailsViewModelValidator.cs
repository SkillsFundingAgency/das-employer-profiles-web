using FluentValidation;
using SFA.DAS.Employer.Profiles.Web.Extensions;
using SFA.DAS.Employer.Profiles.Web.Models;

namespace SFA.DAS.Employer.Profiles.Web.Validations;

public sealed class EditUserDetailsViewModelValidator : AbstractValidator<EditUserDetailsModel>
{
    public EditUserDetailsViewModelValidator()
    {
        RuleFor(x => x.FirstName).ValidName("First name");
        RuleFor(x => x.LastName).ValidName("Last name");
    }
}

