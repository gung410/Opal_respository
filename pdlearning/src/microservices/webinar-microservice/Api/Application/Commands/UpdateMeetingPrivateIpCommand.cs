using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands
{
    public class UpdateMeetingPrivateIpCommand : BaseThunderCommand
    {
        public Guid MeetingId { get; set; }

        public string BBBServerPrivateIp { get; set; }
    }
}
