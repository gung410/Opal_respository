using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class LearnerTakeAttendanceRequest
    {
        public Guid SessionId { get; set; }

        public string SessionCode { get; set; }
    }
}
