using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SetAbsentForMissingInfoAttendanceTrackingCommand : BaseThunderCommand
    {
        public DateTime? ForSessionStartAfter { get; set; }

        public DateTime? ForSessionStartBefore { get; set; }
    }
}
