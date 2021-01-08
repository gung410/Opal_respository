using System;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    public class SurveyResponse : BaseEntity
    {
        public Guid FormId { get; set; }

        public Guid UserId { get; set; }

        public DateTime? AttendanceTime { get; set; }

        public DateTime? SubmittedTime { get; set; }
    }
}
