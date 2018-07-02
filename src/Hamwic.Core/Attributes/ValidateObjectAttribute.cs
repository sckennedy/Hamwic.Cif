using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hamwic.Core.Entities;

namespace Hamwic.Core.Attributes
{
    public class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);

            Validator.TryValidateObject(value, context, results, true);

            if (results.Count == 0) return ValidationResult.Success;

            var compositeResults = new CompositeValidationResult(
                $"Validation for {validationContext.DisplayName} failed!",
                new[] {validationContext.DisplayName});

            results.ForEach(compositeResults.AddResult);

            return compositeResults;
        }
    }
}