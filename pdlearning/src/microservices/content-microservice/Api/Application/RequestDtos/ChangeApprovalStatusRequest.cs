using System;
using Microservice.Content.Domain.Enums;

namespace Microservice.Content.Application.RequestDtos
{
    public class ChangeApprovalStatusRequest
    {
        public Guid Id { get; set; }

        public DigitalContentStatus Status { get; set; }

        public string Comment { get; set; }
    }
}
