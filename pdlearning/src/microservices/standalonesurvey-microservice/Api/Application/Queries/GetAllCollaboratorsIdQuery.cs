using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetAllCollaboratorsIdQuery : BaseStandaloneSurveyQuery<List<Guid>>
    {
        public GetAllAccessRightIdsRequest Request { get; set; }
    }
}
