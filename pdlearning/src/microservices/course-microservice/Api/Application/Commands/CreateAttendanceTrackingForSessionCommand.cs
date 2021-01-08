using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class CreateAttendanceTrackingForSessionCommand : BaseThunderCommand
    {
        public Guid SessionId { get; set; }
    }
}
