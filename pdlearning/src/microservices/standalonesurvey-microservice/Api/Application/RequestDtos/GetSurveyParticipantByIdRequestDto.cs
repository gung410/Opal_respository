using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class GetSurveyParticipantByIdRequestDto
    {
        public Guid FormOriginalObjectId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
