using System;
using FluentValidation.Results;

namespace Application.Exceptions
{
    public class ModelValidationException : ApplicationException
    {
        public List<string> ValdationErrors { get; set; }

        public ModelValidationException(ValidationResult validationResult)
        {
            ValdationErrors = new List<string>();

            foreach (var validationError in validationResult.Errors)
            {
                ValdationErrors.Add("Property " + validationError.PropertyName + " failed validation. Error was: " + validationError.ErrorMessage);
            }
        }
    }
}
