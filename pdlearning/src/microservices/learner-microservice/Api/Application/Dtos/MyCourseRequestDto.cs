using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class MyCourseRequestDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public MyCourseStatus Status { get; set; }

        public LearningCourseType CourseType { get; set; }

        public Guid ResultId { get; set; }

        public RegistrationStatus MyRegistrationStatus { get; set; }
    }
}
