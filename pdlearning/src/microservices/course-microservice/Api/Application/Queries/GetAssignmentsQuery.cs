using System;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAssignmentsQuery : BaseThunderQuery<PagedResultDto<AssignmentModel>>
    {
        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public bool IncludeQuizForm { get; set; }

        public AssignmentsFilterType FilterType { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
