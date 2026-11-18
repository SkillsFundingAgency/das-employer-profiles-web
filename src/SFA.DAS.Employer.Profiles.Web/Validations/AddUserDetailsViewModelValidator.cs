using FluentValidation;
using SFA.DAS.Employer.Profiles.Web.Extensions;
using SFA.DAS.Employer.Profiles.Web.Models;

namespace SFA.DAS.Employer.Profiles.Web.Validations;

public sealed class AddUserDetailsViewModelValidator : AbstractValidator<AddUserDetailsModel>
{
    public AddUserDetailsViewModelValidator()
    {
        RuleFor(x => x.FirstName).ValidName("First name");
        RuleFor(x => x.LastName).ValidName("Last name");
    }
}

