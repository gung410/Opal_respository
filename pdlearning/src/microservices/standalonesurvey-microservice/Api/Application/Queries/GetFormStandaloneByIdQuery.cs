using System;
using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormStandaloneByIdQuery : BaseStandaloneSurveyQuery<SurveyWithQuestionsModel>
    {
        public Guid FormOriginalObjectId { get; set; }

        public Guid UserId { get; set; }
    }
}
