using System;
using Microservice.Badge.Application.Consumers.Dtos;
using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class RegistrationUpdatedMessage
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public Guid CourseId { get; init; }

        public Guid ClassRunId { get; init; }

        public LearningStatus LearningStatus { get; init; }

        public DateTime? ChangedDate { get; init; }

        public ClassRunDto ClassRun { get; init; }

        public CourseDto Course { get; init; }
    }
}
