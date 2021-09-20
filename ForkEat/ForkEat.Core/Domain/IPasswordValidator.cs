using System.ComponentModel.DataAnnotations;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace ForkEat.Core.Domain
{
    public interface IPasswordValidator
    {
        public bool IsValid { get; set; }
        public ValidationResult Validate(User user);
    }
}