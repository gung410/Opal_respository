using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetParticipantAssignmentTracksRequest : PagedResultRequestDto
    {
        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public Guid? AssignmentId { get; set; }

        public bool? ForCurrentUser { get; set; }

        public bool IncludeQuizAssignmentFormAnswer { get; set; }

        public string SearchText { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
