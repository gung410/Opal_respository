using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Questions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class SearchFormQuestionAnswerStatisticsQueryHandler : BaseThunderQueryHandler<SearchFormQuestionAnswerStatisticsQuery, PagedResultDto<SurveyQuestionAnswerStatisticsModel>>
    {
        private readonly IRepository<SurveyQuestionAnswer> _formQuestionAnswerRepository;
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;

        public SearchFormQuestionAnswerStatisticsQueryHandler(
            IRepository<SurveyQuestionAnswer> formQuestionAnswerRepository,
            IRepository<SurveyQuestion> formQuestionRepository)
        {
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
            _formQuestionRepository = formQuestionRepository;
        }

        protected override async Task<PagedResultDto<SurveyQuestionAnswerStatisticsModel>> HandleAsync(SearchFormQuestionAnswerStatisticsQuery query, CancellationToken cancellationToken)
        {
            var formQuestion = await _formQuestionRepository.GetAll().FirstOrDefaultAsync(p => p.Id == query.FormQuestionId, cancellationToken);
            if (formQuestion == null)
            {
                throw new SurveyQuestionNotFoundException();
            }

            var formAnswerQuery = _formQuestionAnswerRepository.GetAll()
                .Where(p =>
                    p.SurveyQuestionId.Equals(query.FormQuestionId)
                    && p.AnswerValue != null)
                .Select(p => new { p.CreatedBy, p.CreatedDate, FormQuestionId = p.SurveyQuestionId, p.AnswerValue });
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

            List<SurveyQuestionAnswerStatisticsModel> formAnswerStatisticList = new List<SurveyQuestionAnswerStatisticsModel>();
            foreach (QuestionOption option in formQuestion.Options)
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
                var statistics = new SurveyQuestionAnswerStatisticsModel
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

            return new PagedResultDto<SurveyQuestionAnswerStatisticsModel>(totalCount, entityModels);
        }
    }
}
