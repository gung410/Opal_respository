using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class TransferOwnershipRequest : HasSubModuleInfoBase
    {
        public Guid ObjectId { get; set; }

        public Guid NewOwnerId { get; set; }
    }
}
