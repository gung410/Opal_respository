using System;
using System.Collections.Generic;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class CreateAccessRightRequest : HasSubModuleInfoBase
    {
        public Guid? Id { get; set; }

        public IEnumerable<Guid> UserIds { get; set; }

        public Guid OriginalObjectId { get; set; }
    }
}
