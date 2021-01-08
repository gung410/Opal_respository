using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeAttendancesStatusCommand : BaseThunderCommand
    {
        public Guid SessionId { get; set; }

        public IEnumerable<Guid> AttendanceTrackingIds { get; set; }

        public AttendanceTrackingStatus Status { get; set; }
    }
}
