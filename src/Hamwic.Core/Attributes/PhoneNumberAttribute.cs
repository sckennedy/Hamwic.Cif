using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hamwic.Core.Attributes
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public static readonly Regex ValidationRegex = new Regex(@"^[\d\s]{10,13}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public bool Required { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var testValue = (value as string ?? string.Empty) ;

            if (!Required && string.IsNullOrEmpty(testValue))
                return ValidationResult.Success;

            if (!ValidationRegex.IsMatch(testValue))
                return new ValidationResult(
                    $"{validationContext.DisplayName} is not a valid telephone number. " +
                    "Enter between 10 and 13 digits.",
                    new[] {validationContext.MemberName});

            return ValidationResult.Success;
        }
    }
}