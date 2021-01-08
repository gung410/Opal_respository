using System;
using Microservice.Course.Application.Enums;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetAssignmentsRequest : PagedResultRequestDto
    {
        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public AssignmentsFilterType FilterType { get; set; }

        public bool IncludeQuizForm { get; set; }
    }
}
