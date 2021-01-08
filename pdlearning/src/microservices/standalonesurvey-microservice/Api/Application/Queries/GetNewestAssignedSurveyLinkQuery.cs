using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetNewestAssignedSurveyLinkQuery : BaseStandaloneSurveyQuery<AssignedLinkModel>
    {
        public Guid User { get; set; }
    }
}
