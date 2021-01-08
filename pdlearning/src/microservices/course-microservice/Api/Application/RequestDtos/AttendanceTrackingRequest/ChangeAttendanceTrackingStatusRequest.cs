using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeAttendanceTrackingStatusRequest
    {
        public Guid SessionId { get; set; }

        public List<Guid> Ids { get; set; }

        public AttendanceTrackingStatus Status { get; set; }
    }
}
