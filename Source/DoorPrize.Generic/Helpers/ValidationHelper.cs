using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoorPrize.Generic
{
    public static class ValidationHelper
    {
        public static void Validate(object @object)
        {
            List<ValidationResult> results;

            if (!ValidationHelper.TryValidate(@object, out results))
                throw new ValidationError(results);
        }

        public static bool TryValidate(
            object @object, out List<ValidationResult> results)
        {
            var context = new ValidationContext(
                @object, serviceProvider: null, items: null);

            var collection = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(
                @object, context, collection, validateAllProperties: true);

            results = new List<ValidationResult>(collection);

            return isValid;
        }

        public static ValidationResult GetValidationResult(
            Func<bool> isValid, string memberName, string suffix)
        {
            if (!isValid())
            {
                return new ValidationResult(string.Format(
                    "The {0} field must {1}.", memberName, suffix));
            }

            return ValidationResult.Success;
        }
    }
}
