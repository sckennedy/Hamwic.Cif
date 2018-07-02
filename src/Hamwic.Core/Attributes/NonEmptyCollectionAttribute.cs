using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Hamwic.Core.Attributes
{
    public class NonEmptyCollectionAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var testCollection = value as IEnumerable;

            if (testCollection == null || !testCollection.GetEnumerator().MoveNext())
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName),
                    new[] {validationContext.MemberName});

            return ValidationResult.Success;
        }
    }
}