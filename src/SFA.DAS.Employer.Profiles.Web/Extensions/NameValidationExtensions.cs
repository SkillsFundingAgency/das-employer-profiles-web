using FluentValidation;
using System.Linq.Expressions;

namespace SFA.DAS.Employer.Profiles.Web.Extensions
{
    public static class NameValidationExtensions
    {
        private const string ValidNameRegex = @"^[\p{L}\p{M}]+(?:[ '-][\p{L}\p{M}]+)*$";

        public static IRuleBuilderOptions<T, string?> ValidName<T>(this IRuleBuilderInitial<T, string?> ruleBuilder, string name)
        {
            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"Enter your {name.ToLowerInvariant()}")
                .Matches(ValidNameRegex).WithMessage($"{name} must only include letters, spaces, hyphens and apostrophes");
        }
    }
}
