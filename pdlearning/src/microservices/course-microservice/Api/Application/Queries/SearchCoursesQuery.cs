using System;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class SearchCoursesQuery : BaseThunderQuery<PagedResultDto<CourseModel>>
    {
        public Guid? CoursePlanningCycleId { get; set; }

        public string SearchText { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }

        public SearchCourseType SearchType { get; set; }

        public bool CheckCourseContent { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
