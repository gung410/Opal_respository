using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Queries;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Services
{
    public class FormQuestionAnswerApplicationService : BaseApplicationService
    {
        public FormQuestionAnswerApplicationService(IThunderCqrs thunderCqrs) : base(thunderCqrs)
        {
        }

        public async Task<IEnumerable<FormQuestionAnswerStatisticsModel>> GetStatisticsByQuestionId(Guid? formQuestionId)
        {
            var searchFormAnswersQueryResult = await ThunderCqrs.SendQuery(new SearchFormQuestionAnswerStatisticsQuery
            {
                FormQuestionId = formQuestionId
            });
            return searchFormAnswersQueryResult.Items;
        }
    }
}
