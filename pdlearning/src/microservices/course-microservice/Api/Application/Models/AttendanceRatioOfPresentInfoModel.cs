using System;

namespace Microservice.Course.Application.Models
{
    public class AttendanceRatioOfPresentInfo
    {
        public Guid RegistrationId { get; set; }

        public int TotalSessions { get; set; }

        public int PresentSessions { get; set; }
    }
}
