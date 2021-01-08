using System;
using System.Text.Json.Serialization;

namespace Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads
{
    public class LearnerActivityLecturePayload
    {
        public Guid CourseId { get; set; }

        [JsonPropertyName("lectureid")]
        public Guid LectureId { get; set; }
    }
}
