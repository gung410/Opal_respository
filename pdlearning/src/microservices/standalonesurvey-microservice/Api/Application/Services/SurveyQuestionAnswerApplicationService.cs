using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Queries;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Services
{
    public class SurveyQuestionAnswerApplicationService : BaseApplicationService
    {
        public SurveyQuestionAnswerApplicationService(IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
        }

        public async Task<IEnumerable<SurveyQuestionAnswerStatisticsModel>> GetStatisticsByQuestionId(Guid? formQuestionId)
        {
            var searchFormAnswersQueryResult = await ThunderCqrs.SendQuery(new SearchFormQuestionAnswerStatisticsQuery
            {
                FormQuestionId = formQuestionId
            });
            return searchFormAnswersQueryResult.Items;
        }
    }
}
