using System;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Commands
{
    public class UpdateMeetingInfoBBBServerIdCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public Guid? BBBServerId { get; set; }
    }
}
