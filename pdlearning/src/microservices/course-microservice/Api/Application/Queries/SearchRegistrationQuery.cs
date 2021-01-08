using System;
using System.Collections.Generic;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class SearchRegistrationQuery : BaseThunderQuery<PagedResultDto<RegistrationModel>>
    {
        public Guid? CourseId { get; set; }

        public List<Guid> ClassRunIds { get; set; }

        public Guid? ExcludeAssignedAssignmentId { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }

        public SearchRegistrationType SearchType { get; set; }

        public bool IncludeUserInfo { get; set; }

        public bool IncludeCourseClassRun { get; set; }

        public string SearchText { get; set; }

        /// <summary>
        /// Whether search text is used to filter for course name or not.
        /// </summary>
        public bool ApplySearchTextForCourse { get; set; }

        public CommonFilter UserFilter { get; set; }

        public CommonFilter Filter { get; set; }

        public bool HasUserFilter()
        {
            return (UserFilter?.ContainFilters != null && UserFilter.ContainFilters.Count > 0) ||
                    (UserFilter?.FromToFilters != null && UserFilter.FromToFilters.Count > 0);
        }
    }
}
