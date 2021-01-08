using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetParticipantAssignmentTracksQuery : BaseThunderQuery<PagedResultDto<ParticipantAssignmentTrackModel>>
    {
        public Guid? CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? AssignmentId { get; set; }

        public bool? ForCurrentUser { get; set; }

        public IEnumerable<Guid> RegistrationIds { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }

        public bool IncludeQuizAssignmentFormAnswer { get; set; }

        public string SearchText { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
