using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetVersionTrackingFormDataByIdQuery : BaseStandaloneSurveyQuery<VersionTrackingSurveyDataModel>
    {
        public Guid FormId { get; set; }

        public Guid UserId { get; set; }

        public bool OnlyPublished { get; set; }
    }
}
