using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetAttendanceRatioOfPresentsRequest
    {
        public Guid ClassRunId { get; set; }

        public Guid[] RegistrationIds { get; set; }
    }
}
