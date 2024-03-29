using System;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Versioning.Application.Model;

namespace Microservice.StandaloneSurvey.Versioning.Application.Queries
{
    public class GetActiveVersionsQuery : BaseStandaloneSurveyQuery<VersionTrackingModel>
    {
        public Guid UserId { get; set; }

        public Guid OriginalObjectId { get; set; }
    }
}
