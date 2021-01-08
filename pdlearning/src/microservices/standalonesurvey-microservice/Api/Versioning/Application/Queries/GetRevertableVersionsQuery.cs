using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Versioning.Application.Model;

namespace Microservice.StandaloneSurvey.Versioning.Application.Queries
{
    public class GetRevertableVersionsQuery : BaseStandaloneSurveyQuery<List<VersionTrackingModel>>
    {
        public Guid UserId { get; set; }

        public Guid OriginalObjectId { get; set; }
    }
}
