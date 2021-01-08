using System;
using Microservice.Course.Application.Enums;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchCoursesRequest : PagedResultRequestDto
    {
        public string SearchText { get; set; }

        public SearchCourseType SearchType { get; set; }

        public bool CheckCourseContent { get; set; }

        public Guid? CoursePlanningCycleId { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
