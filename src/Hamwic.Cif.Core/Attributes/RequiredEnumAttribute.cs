using System.ComponentModel.DataAnnotations;

namespace Hamwic.Cif.Core.Attributes
{
    public class RequiredEnumAttribute : RequiredAttribute
    {
        public int NullOrEmptyValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || (int) value == NullOrEmptyValue)
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName),
                    new[] {validationContext.MemberName});

            return ValidationResult.Success;
        }
    }
}