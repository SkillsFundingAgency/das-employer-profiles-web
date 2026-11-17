using FluentValidation;
using SFA.DAS.Employer.Profiles.Web.Extensions;
using SFA.DAS.Employer.Profiles.Web.Models;

namespace SFA.DAS.Employer.Profiles.Web.Validations;

public sealed class AddUserDetailsViewModelValidator : AbstractValidator<AddUserDetailsModel>
{
    public AddUserDetailsViewModelValidator()
    {
        NameValidationExtensions.AddNameRules(
            this, x => x.FirstName, "First name");

        NameValidationExtensions.AddNameRules(
            this, x => x.LastName, "Last name");
    }
}

