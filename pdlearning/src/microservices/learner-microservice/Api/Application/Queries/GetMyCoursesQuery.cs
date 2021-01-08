using System;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyCoursesQuery : BaseThunderQuery<PagedResultDto<CourseModel>>, IPagedResultAware
    {
        public string SearchText { get; set; }

        public MyLearningStatus StatusFilter { get; set; }

        public string OrderBy { get; set; }

        public Guid CourseId { get; set; }

        public LearningCourseType CourseType { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
