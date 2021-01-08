using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class SearchFormAnswersQueryHandler : BaseThunderQueryHandler<SearchSurveyAnswersQuery, PagedResultDto<SurveyAnswerModel>>
    {
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveyQuestionAnswer> _formQuestionAnswerRepository;
        private readonly IRepository<SurveyAnswer> _formAnswerRepository;

        public SearchFormAnswersQueryHandler(
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveyQuestionAnswer> formQuestionAnswerRepository,
            IRepository<SurveyAnswer> formAnswerRepository)
        {
            _formRepository = formRepository;
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
            _formAnswerRepository = formAnswerRepository;
        }

        protected override async Task<PagedResultDto<SurveyAnswerModel>> HandleAsync(SearchSurveyAnswersQuery query, CancellationToken cancellationToken)
        {
            var form = await _formRepository.FirstOrDefaultAsync(p => p.Id.Equals(query.SurveyId));

            if (form == null)
            {
                throw new SurveyAccessDeniedException();
            }

            var now = Clock.Now;
            var formAnswerQuery = _formAnswerRepository.GetAll()
                .Where(p =>
                    p.FormId.Equals(query.SurveyId)
                    && p.ResourceId.Equals(query.ResourceId)
                    && p.OwnerId == query.UserId)
                .WhereIf(
                    query.IsSubmitted.HasValue,
                    p => query.IsSubmitted == true ? p.SubmitDate != null : p.SubmitDate == null)
                .WhereIf(
                    query.IsCompleted.HasValue,
                    p => p.IsCompleted == query.IsCompleted)
                .WhereIf(
                    query.BeforeDueDate.HasValue && form.DueDate.HasValue,
                    p => now < form.DueDate);

            var totalCount = await formAnswerQuery.CountAsync(cancellationToken);

            formAnswerQuery = ApplyPaging(formAnswerQuery, query.PagedInfo);

            var formQuestionAnswersQuery = from fqa in _formQuestionAnswerRepository.GetAll()
                                           join fa in formAnswerQuery on fqa.SurveyAnswerId equals fa.Id
                                           select fqa;

            var formAnswers = await formAnswerQuery
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync(cancellationToken);

            var formQuestionAnswersDic = (await formQuestionAnswersQuery.ToListAsync(cancellationToken))
                .GroupBy(p => p.SurveyAnswerId, p => p)
                .ToDictionary(p => p.Key, p => p.ToList());

            var entityModels = formAnswers
                .Select(p => new SurveyAnswerModel(
                    p,
                    formQuestionAnswersDic.ContainsKey(p.Id) ? formQuestionAnswersDic[p.Id] : new List<SurveyQuestionAnswer>()))
                .ToList();

            return new PagedResultDto<SurveyAnswerModel>(totalCount, entityModels);
        }
    }
}
