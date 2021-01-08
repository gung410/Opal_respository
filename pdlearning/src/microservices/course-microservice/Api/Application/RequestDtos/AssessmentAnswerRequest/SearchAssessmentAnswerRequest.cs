using System;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class SearchAssessmentAnswerRequest : PagedResultRequestDto
    {
        public Guid? ParticipantAssignmentTrackId { get; set; }

        public Guid? UserId { get; set; }

        public string SearchText { get; set; }

        public CommonFilter Filter { get; set; }
    }
}
