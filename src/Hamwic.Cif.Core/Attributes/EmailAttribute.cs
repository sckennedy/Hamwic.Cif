using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Hamwic.Cif.Core.Attributes
{
    public class EmailAttribute : ValidationAttribute
    {
        public static readonly Regex ValidationRegex =
            new Regex(
                @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public bool Required { get; set; }

        public static bool IsValid(string value)
        {
            try
            {
                return new System.Net.Mail.MailAddress(value ?? string.Empty).Address == (value ?? string.Empty);
            }
            catch
            {
                return false;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var testValue = value as string;

            if (!Required && string.IsNullOrEmpty(testValue))
                return ValidationResult.Success;

            if (Required && (string.IsNullOrEmpty(testValue) || string.IsNullOrWhiteSpace(testValue)))
                return
                    new ValidationResult(
                        $"{validationContext.DisplayName} is a required email address",
                        new[] { validationContext.MemberName });

            if (IsValid(testValue))
                return ValidationResult.Success;

            return new ValidationResult(
                $"{validationContext.DisplayName} is not a valid email address",
                        new[] { validationContext.MemberName });
        }
    }
}