using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class InitAttendanceTrackingForSessionCommand : BaseThunderCommand
    {
        public Guid SessionId { get; set; }

        public bool ForDailyTracking { get; set; }
    }
}
