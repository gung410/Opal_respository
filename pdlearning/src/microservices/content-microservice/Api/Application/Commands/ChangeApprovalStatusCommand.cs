using System;
using Microservice.Content.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class ChangeApprovalStatusCommand : BaseThunderCommand
    {
        public Guid ContentId { get; set; }

        public DigitalContentStatus Status { get; set; }

        public string Comment { get; set; }

        public Guid UserId { get; set; }
    }
}
