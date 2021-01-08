using System;
using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class ReEnrollCourseCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public List<Guid> LectureIds { get; set; }

        public LearningCourseType CourseType { get; set; }
    }
}
