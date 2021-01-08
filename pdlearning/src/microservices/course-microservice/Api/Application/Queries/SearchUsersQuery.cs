using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Queries
{
    public class SearchUsersQuery : BaseThunderQuery<PagedResultDto<UserModel>>
    {
        public string SearchText { get; set; }

        public SearchUsersQueryForCourseInfo CanApplyForCourse { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }

    public class SearchUsersQueryForCourseInfo
    {
        public Guid CourseId { get; set; }

        public bool FollowCourseTargetParticipant { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
