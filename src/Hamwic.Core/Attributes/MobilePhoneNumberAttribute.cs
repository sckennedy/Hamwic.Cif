using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hamwic.Core.Attributes
{
    public class MobilePhoneNumberAttribute : ValidationAttribute
    {
        public const string MobilePhoneNumberRegexPattern =
            @"^(?:447)(?:[03456789]\d{8})$";

        public static readonly Regex ValidationRegex = new Regex(MobilePhoneNumberRegexPattern,
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public bool Required { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var testValue = (value as string) ?? string.Empty;

            if (!Required && string.IsNullOrEmpty(testValue))
                return ValidationResult.Success;

            if (Required && string.IsNullOrEmpty(testValue) || !ValidationRegex.IsMatch(testValue))
                return new ValidationResult("Invalid mobile phone number",
                    new[] {validationContext.MemberName});

            return ValidationResult.Success;
        }
    }
}