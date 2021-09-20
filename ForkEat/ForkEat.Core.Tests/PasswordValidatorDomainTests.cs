using FluentValidation.TestHelper;
using ForkEat.Core.Domain;
using Xunit;

namespace ForkEat.Core.Tests
{
    public class PasswordValidatorDomainTests
    {
        private PasswordValidator passwordValidator;

        [Fact]
        public void CheckPassword_WithValidPassword_Success()
        {
            passwordValidator = new PasswordValidator();
            var model = new User { Password = "J3SuisV@lid" };
            var result = passwordValidator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(user => user.Password);
        }

        [Fact]
        public void CheckPassword_With6Chars_Error() 
        {
            passwordValidator = new PasswordValidator();
            var model = new User { Password = "1v&liD" };
            var result = passwordValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.Password);
        }
        
        [Fact]
        public void CheckPassword_WithNoSpecialChars_Error() 
        {
            passwordValidator = new PasswordValidator();
            var model = new User { Password = "J3Suis1Valid" };
            var result = passwordValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.Password);
        }
        
        [Fact]
        public void CheckPassword_WithNoDigit_Error() 
        {
            passwordValidator = new PasswordValidator();
            var model = new User { Password = "JeSuisINV@lid" };
            var result = passwordValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.Password);
        }
        
        [Fact]
        public void CheckPassword_WithNoUpperCase_Error() 
        {
            passwordValidator = new PasswordValidator();
            var model = new User { Password = "j3suisinv@lid" };
            var result = passwordValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.Password);
        }
    }
}