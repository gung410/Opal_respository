using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Commands
{
    public class ChangeClassRunRescheduleStatusCommand : BaseThunderCommand
    {
        public List<Guid> Ids { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public List<ChangeClassRunRescheduleStatusCommandSession> RescheduleSessions { get; set; }

        public ClassRunRescheduleStatus RescheduleStatus { get; set; }
    }

    public class ChangeClassRunRescheduleStatusCommandSession
    {
        public Guid? Id { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
