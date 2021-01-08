using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class UpdateCourseStatusRequestDto
    {
        public Guid CourseId { get; set; }

        public MyCourseStatus Status { get; set; }
    }
}
