using System;
using Microservice.Course.Application.Enums;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchRegistrationRequest : PagedResultRequestDto
    {
        public Guid? CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? ExcludeAssignedAssignmentId { get; set; }

        public SearchRegistrationType SearchType { get; set; }

        public string SearchText { get; set; }

        /// <summary>
        /// Whether search text is used to filter for course name or not.
        /// </summary>
        public bool ApplySearchTextForCourse { get; set; }

        public CommonFilter UserFilter { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
