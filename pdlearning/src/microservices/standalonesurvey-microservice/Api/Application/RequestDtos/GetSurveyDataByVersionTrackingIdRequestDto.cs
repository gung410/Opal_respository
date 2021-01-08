using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class GetSurveyDataByVersionTrackingIdRequestDto
    {
        public Guid VersionTrackingId { get; set; }

        public Guid UserId { get; set; }
    }
}
