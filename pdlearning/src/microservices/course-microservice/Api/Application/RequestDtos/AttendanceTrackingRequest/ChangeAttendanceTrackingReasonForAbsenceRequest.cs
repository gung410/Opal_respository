using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeAttendanceTrackingReasonForAbsenceRequest
    {
        public Guid SessionId { get; set; }

        public Guid UserId { get; set; }

        public string Reason { get; set; }

        public IEnumerable<string> Attachment { get; set; }
    }
}
