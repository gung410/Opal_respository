using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Queries;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Services
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
