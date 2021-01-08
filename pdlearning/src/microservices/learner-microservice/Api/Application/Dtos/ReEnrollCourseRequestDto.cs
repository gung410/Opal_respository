using System;
using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class ReEnrollCourseRequestDto
    {
        public Guid CourseId { get; set; }

        public List<Guid> LectureIds { get; set; }

        public LearningCourseType CourseType { get; set; }
    }
}
