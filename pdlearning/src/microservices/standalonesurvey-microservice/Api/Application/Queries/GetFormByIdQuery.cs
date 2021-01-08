using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormByIdQuery : BaseStandaloneSurveyQuery<StandaloneSurveyModel>
    {
        public Guid UserId { get; set; }

        public Guid FormId { get; set; }
    }
}
