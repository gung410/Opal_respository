using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeAttendanceTrackingReasonForAbsenceCommand : BaseThunderCommand
    {
        public Guid SessionId { get; set; }

        public string Reason { get; set; }

        public IEnumerable<string> Attachment { get; set; }

        public Guid UserId { get; set; }
    }
}
