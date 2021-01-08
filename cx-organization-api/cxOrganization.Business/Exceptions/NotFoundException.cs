using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cxPlatform.Core.Exceptions;

namespace cxOrganization.Business.Exceptions
{
    public class NotFoundException : CXValidationException
    {
        public NotFoundException(string message)
            : base(cxExceptionCodes.ERROR_NOT_FOUND, new List<ValidationResult> {new ValidationResult(message)})
        {

        }
    }
}
