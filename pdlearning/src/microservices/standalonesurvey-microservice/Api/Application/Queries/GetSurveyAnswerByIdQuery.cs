using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetSurveyAnswerByIdQuery : BaseStandaloneSurveyQuery<SurveyAnswerModel>
    {
        public Guid UserId { get; set; }

        public Guid SurveyAnswerId { get; set; }
    }
}
