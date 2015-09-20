using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoorPrize.Generic
{
    public class ValidationError : Exception
    {
        public ValidationError(List<ValidationResult> results) :
            base(GetMessage(results))
        {
            Results = results;
        }

        private static string GetMessage(List<ValidationResult> results)
        {
            return string.Format(
                "{0} validation errors were detected.  (See the \"Results\" collection for more details.)",
                results.Count);
        }

        public List<ValidationResult> Results { get; private set; }
    }
}