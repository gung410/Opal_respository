using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Questions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class SearchFormQuestionAnswerStatisticsQueryHandler : BaseThunderQueryHandler<SearchFormQuestionAnswerStatisticsQuery, PagedResultDto<FormQuestionAnswerStatisticsModel>>
    {
        private readonly IRepository<FormQuestionAnswer> _formQuestionAnswerRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;

        public SearchFormQuestionAnswerStatisticsQueryHandler(
            IRepository<FormQuestionAnswer> formQuestionAnswerRepository,
            IRepository<FormQuestion> formQuestionRepository)
        {
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
            _formQuestionRepository = formQuestionRepository;
        }

        protected override async Task<PagedResultDto<FormQuestionAnswerStatisticsModel>> HandleAsync(SearchFormQuestionAnswerStatisticsQuery query, CancellationToken cancellationToken)
        {
            var formQuestion = await _formQuestionRepository.GetAll().FirstOrDefaultAsync(p => p.Id == query.FormQuestionId, cancellationToken);
            if (formQuestion == null)
            {
                throw new FormQuestionNotFoundException();
            }

            var formAnswerQuery = _formQuestionAnswerRepository.GetAll()
                .Where(p =>
                    p.FormQuestionId.Equals(query.FormQuestionId)
                    && p.AnswerValue != null)
                .Select(p => new { p.CreatedBy, p.CreatedDate, p.FormQuestionId, p.AnswerValue });
            var formAnswerDistinctQuery = formAnswerQuery.Select(p => p.CreatedBy).Distinct()
                .SelectMany(key => formAnswerQuery
                    .Where(p => p.CreatedBy == key)
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(1));

            var formAnswerList = await formAnswerDistinctQuery
                .ToListAsync(cancellationToken);

            var answerCount = formAnswerList
                .Select(x => ((JsonElement)x.AnswerValue).ValueKind == JsonValueKind.Array ? ((JsonElement)x.AnswerValue).EnumerateArray().Count() : 1)
                .Sum();

            List<FormQuestionAnswerStatisticsModel> formAnswerStatisticList = new List<FormQuestionAnswerStatisticsModel>();
            foreach (QuestionOption option in formQuestion.Question_Options)
            {
                var statisticsEnumerable = formAnswerList
                        .Where(p =>
                            (((JsonElement)p.AnswerValue).ValueKind == JsonValueKind.Array
                            && JsonConvert.DeserializeObject<List<string>>(((JsonElement)p.AnswerValue).ToString())
                                .Contains(((JsonElement)option.Value).ToString()))
                            ||
                            (((JsonElement)p.AnswerValue).ValueKind == JsonValueKind.String
                            && ((JsonElement)p.AnswerValue).ToString().Equals(((JsonElement)option.Value).ToString())));
                var statisticsCount = statisticsEnumerable.Count();
                var percentage = ((double)statisticsCount / (double)answerCount) * 100;
                var statistics = new FormQuestionAnswerStatisticsModel
                    {
                        AnswerCode = option.Code,
                        AnswerValue = ((JsonElement)option.Value).ToString(),
                        AnswerCount = statisticsCount,
                        AnswerPercentage = double.IsFinite(percentage) ? percentage : 0.0
                    };
                formAnswerStatisticList.Add(statistics);
            }

            var totalCount = formAnswerStatisticList.Count;

            var entityModels = formAnswerStatisticList.ToList();

            return new PagedResultDto<FormQuestionAnswerStatisticsModel>(totalCount, entityModels);
        }
    }
}
