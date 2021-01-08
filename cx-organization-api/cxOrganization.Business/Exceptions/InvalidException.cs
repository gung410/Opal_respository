using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Business.Exceptions
{
    public class InvalidException : CXValidationException
    {
        public InvalidException(string message) 
            : base(cxExceptionCodes.ERROR_CUSTOM, new List<ValidationResult> {new ValidationResult(message)})
        {
        }
    }
}