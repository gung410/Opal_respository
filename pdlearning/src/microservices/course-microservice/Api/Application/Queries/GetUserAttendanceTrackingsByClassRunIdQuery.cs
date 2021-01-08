using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetUserAttendanceTrackingsByClassRunIdQuery : BaseThunderQuery<List<AttendanceTrackingModel>>
    {
        public Guid ClassRunId { get; set; }
    }
}
