using FluentValidation;
using System.Linq.Expressions;

namespace SFA.DAS.Employer.Profiles.Web.Extensions
{
    public static class NameValidationExtensions
    {
        private const string ValidNameRegex = @"^[\p{L}\p{M}]+(?:[ '-][\p{L}\p{M}]+)*$";

        public static void AddNameRules<T>(
        AbstractValidator<T> validator,
        Expression<Func<T, string?>> property,
        string name)
        {
            validator.RuleFor(property)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage($"Enter your {name.ToLowerInvariant()}")
            .Must(value => value == null || value.Any(char.IsLetter))
            .WithMessage($"Enter a valid {name.ToLowerInvariant()}")
            .Matches(ValidNameRegex)
            .WithMessage($"{name} must only include letters, spaces, hyphens and apostrophes");
        }
    }
}
