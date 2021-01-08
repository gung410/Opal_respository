using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class UpdateResponseRequest
    {
        public Guid FormId { get; set; }

        public Guid UserId { get; set; }

        public DateTime? AttendanceTime { get; set; }

        public DateTime? SubmittedTime { get; set; }
    }
}
