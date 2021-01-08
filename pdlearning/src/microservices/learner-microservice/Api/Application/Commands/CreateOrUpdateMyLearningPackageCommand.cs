using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class CreateOrUpdateMyLearningPackageCommand : BaseThunderCommand
    {
        /// <summary>
        /// Id of LectureInMyCourse. Only use when the Learning Package is inside a Course.
        /// Combine with UserId to specify MyLearningPackage.
        /// </summary>
        public Guid? MyLectureId { get; set; }

        /// <summary>
        /// Id of MyDigitalContent. Only use when the Learning Package is inside a standalone DigitalContent.
        /// Combine with UserId to specify MyLearningPackage.
        /// </summary>
        public Guid? MyDigitalContentId { get; set; }

        public LearningPackageType Type { get; set; }

        public string State { get; set; }

        public string LessonStatus { get; set; }

        public string CompletionStatus { get; set; }

        public string SuccessStatus { get; set; }

        public int? TimeSpan { get; set; }
    }
}
