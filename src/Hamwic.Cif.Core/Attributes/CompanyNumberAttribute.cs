using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hamwic.Cif.Core.Attributes
{
    public class CompanyNumberAttribute : ValidationAttribute
    {
        public static readonly Regex ValidationRegex =
            new Regex(
                @"(^[0-9]{7,8}$|^[SC]{2}[0-9]{6}$)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public const string SampleText = "1234567 or 12345678 or SC123456";

        public bool Required { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var testValue = value as string;

            if (!Required && string.IsNullOrEmpty(testValue))
                return ValidationResult.Success;

            if (Required && (string.IsNullOrEmpty(testValue) || string.IsNullOrWhiteSpace(testValue)))
                return
                    new ValidationResult(
                        string.Format("{0} is a required Company number", validationContext.DisplayName),
                        new[] { validationContext.MemberName });

            if (!ValidationRegex.IsMatch(testValue ?? string.Empty))
                return
                    new ValidationResult(
                        string.Format("{0} is not a valid Company Number address", validationContext.DisplayName),
                        new[] {validationContext.MemberName});

            return ValidationResult.Success;
        }
    }
}