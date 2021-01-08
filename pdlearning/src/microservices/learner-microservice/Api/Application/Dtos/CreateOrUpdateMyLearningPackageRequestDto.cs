using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class CreateOrUpdateMyLearningPackageRequestDto
    {
        public Guid? MyLectureId { get; set; }

        public Guid? MyDigitalContentId { get; set; }

        public LearningPackageType Type { get; set; }

        public object State { get; set; }

        public string LessonStatus { get; set; }

        public string CompletionStatus { get; set; }

        public string SuccessStatus { get; set; }

        public int? TimeSpan { get; set; }
    }
}
