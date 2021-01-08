using System;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Versioning.Application.Model;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Versioning.Application.Queries
{
    public class GetVersionTrackingByIdQuery : BaseStandaloneSurveyQuery<VersionTrackingModel>
    {
        public Guid UserId { get; set; }

        public Guid VersionId { get; set; }
    }
}
