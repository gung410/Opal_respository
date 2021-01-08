using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ValidateConfirmBlockoutDateLogic : BaseBusinessLogic
    {
        public ValidateConfirmBlockoutDateLogic(IUserContext userContext) : base(userContext)
        {
        }

        public Validation Validate(List<BlockoutDate> blockoutDates)
        {
            if (blockoutDates.Any(p => p.Status == BlockoutDateStatus.Draft))
            {
                return Validation.Invalid("Unable to confirm the list of block-out dates because there are still Draft block-out date.");
            }

            return Validation.Valid();
        }
    }
}
