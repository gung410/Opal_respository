using System;

namespace Microservice.Content.Application.RequestDtos
{
    public class TransferOwnershipRequest
    {
        public Guid ObjectId { get; set; }

        public Guid NewOwnerId { get; set; }
    }
}
