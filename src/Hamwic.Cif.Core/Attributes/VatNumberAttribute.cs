using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hamwic.Cif.Core.Attributes
{
    public class VatNumberAttribute : ValidationAttribute
    {
        public static readonly Regex ValidationRegex =
            new Regex(
                @"^(GB){0,1}([1-9][0-9]{2}[0-9]{4}[0-9]{2})|([1-9][0-9]{2}[0-9]{4}[0-9]{2}[0-9]{3})|((GD|HA)[0-9]{3})$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public const string SampleText = "GB123456789 or GB123456789012";

        public bool Required { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var testValue = value as string;

            if (!Required && string.IsNullOrEmpty(testValue))
                return ValidationResult.Success;

            if (Required && (string.IsNullOrEmpty(testValue) || string.IsNullOrWhiteSpace(testValue)))
                return
                    new ValidationResult(
                        string.Format("{0} is a required VAT number", validationContext.DisplayName),
                        new[] { validationContext.MemberName });

            if (!ValidationRegex.IsMatch(testValue ?? string.Empty))
                return
                    new ValidationResult(
                        string.Format("{0} is not a valid Vat Number address", validationContext.DisplayName),
                        new[] {validationContext.MemberName});

            return ValidationResult.Success;
        }
    }
}