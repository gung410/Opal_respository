using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Learner.Application.Dtos
{
    public class GetMyCoursesRequestDto : PagedResultRequestDto
    {
        public MyLearningStatus StatusFilter { get; set; }

        public string OrderBy { get; set; }

        public Guid CourseId { get; set; }

        public LearningCourseType CourseType { get; set; }
    }
}
