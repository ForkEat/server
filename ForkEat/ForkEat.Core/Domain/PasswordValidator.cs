using FluentValidation;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace ForkEat.Core.Domain
{
    public class PasswordValidator : AbstractValidator<User>, IPasswordValidator
    {
        public bool IsValid { get; set; }
        
        public PasswordValidator()
        {
            RuleFor(user => user.Password).NotEmpty();
            RuleFor(user => user.Password).MinimumLength(8);
            RuleFor(user => user.Password).Matches("[A-Z]").WithMessage("'{PropertyName}' must contain one or more uppercase.");
            RuleFor(user => user.Password).Matches("[a-z]").WithMessage("'{PropertyName}' must contain one or more lowercase.");
            RuleFor(user => user.Password).Matches("[0-9]").WithMessage("'{PropertyName}' must contain one or more number.");
            RuleFor(user => user.Password).Matches("[\"!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("'{PropertyName}' must contain one or more special character.");
        }

        public ValidationResult Validate(User user)
        {
            var result = base.Validate(user);
            IsValid = result.IsValid;
            return result;
        }
    }
}