using System;

namespace Microservice.LnaForm.Application.RequestDtos
{
    public class TransferOwnershipRequest
    {
        public Guid ObjectId { get; set; }

        public Guid NewOwnerId { get; set; }
    }
}