using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Thunder.Platform.Core.Exceptions;

namespace Thunder.Platform.AspNetCore.Validation
{
    public static class ModelStateErrorBuilder
    {
        public static ErrorInfo BuildErrorResponse([NotNull] this ModelStateDictionary modelStateDictionary)
        {
            if (modelStateDictionary == null)
            {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }

            var error = new ErrorInfo
            {
                Code = nameof(DataValidationException),
                Message = "There is an error when validating input data.",
                Details = new List<ErrorInfo>()
            };

            foreach (string modelStateKey in modelStateDictionary.Keys)
            {
                ModelStateEntry property = modelStateDictionary[modelStateKey];
                var detailError = new ErrorInfo
                {
                    Code = nameof(DataValidationException),
                    Message = $"Property {modelStateKey} has errors when validating.",
                    Target = modelStateKey,
                    Details = new List<ErrorInfo>()
                };

                foreach (ModelError propertyError in property.Errors)
                {
                    detailError.Details.Add(new ErrorInfo
                    {
                        Message = propertyError.ErrorMessage
                    });
                }

                error.Details.Add(detailError);
            }

            return error;
        }
    }
}
