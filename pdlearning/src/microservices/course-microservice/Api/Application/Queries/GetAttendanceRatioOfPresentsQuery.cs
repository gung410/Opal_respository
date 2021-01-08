using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAttendanceRatioOfPresentsQuery : BaseThunderQuery<List<AttendanceRatioOfPresentInfo>>
    {
        public Guid ClassRunId { get; set; }

        public Guid[] RegistrationIds { get; set; }
    }
}
