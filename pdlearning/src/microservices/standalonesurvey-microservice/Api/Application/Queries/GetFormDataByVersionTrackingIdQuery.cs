using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormDataByVersionTrackingIdQuery : BaseStandaloneSurveyQuery<VersionTrackingSurveyDataModel>
    {
        public Guid VersionTrackingId { get; set; }

        public Guid UserId { get; set; }
    }
}
