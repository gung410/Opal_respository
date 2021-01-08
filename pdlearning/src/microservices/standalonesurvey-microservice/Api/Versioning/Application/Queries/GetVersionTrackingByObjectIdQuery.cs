using System;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Versioning.Application.Model;
using Microservice.StandaloneSurvey.Versioning.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Versioning.Application.Queries
{
    public class GetVersionTrackingByObjectIdQuery : BaseStandaloneSurveyQuery<PagedResultDto<VersionTrackingModel>>
    {
        public Guid UserId { get; set; }

        public GetVersionTrackingByObjectIdRequest Request { get; set; }
    }
}
