using System;
using Thunder.Platform.Core.Application.Dtos;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.RequestDtos
{
    public class SearchCourseUsersRequest : PagedResultRequestDto
    {
        public string SearchText { get; set; }

        public SearchUsersQueryForCourseInfo ForCourse { get; set; }
    }

    public class SearchUsersQueryForCourseInfo
    {
        public Guid CourseId { get; set; }

        public bool FollowCourseTargetParticipant { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
