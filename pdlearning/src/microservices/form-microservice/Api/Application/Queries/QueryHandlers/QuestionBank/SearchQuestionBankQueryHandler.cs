using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Form.Application.Queries
{
    public class SearchQuestionBankQueryHandler : BaseQueryHandler<SearchQuestionBankQuery, PagedResultDto<QuestionBankModel>>
    {
        private readonly IRepository<QuestionBank> _questionBankRepository;
        private readonly IRepository<QuestionGroup> _questionGroupRepository;

        public SearchQuestionBankQueryHandler(
            IRepository<QuestionBank> questionBankRepository,
            IRepository<QuestionGroup> questionGroupRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _questionBankRepository = questionBankRepository;
            _questionGroupRepository = questionGroupRepository;
        }

        protected override async Task<PagedResultDto<QuestionBankModel>> HandleAsync(SearchQuestionBankQuery query, CancellationToken cancellationToken)
        {
            var questionBankQuery = _questionBankRepository.GetAll().Where(questionBank => questionBank.CreatedBy == CurrentUserId);

            questionBankQuery = questionBankQuery.WhereIf(
                !string.IsNullOrEmpty(query.Title),
                questionBank => EF.Functions.Contains(questionBank.Title, "\"" + query.Title + "*\""));

            questionBankQuery = questionBankQuery.WhereIf(
                query.QuestionGroupIds.Any(),
                questionBank => questionBank.QuestionGroupId.HasValue &&
                                query.QuestionGroupIds.Contains(questionBank.QuestionGroupId.Value));

            questionBankQuery = questionBankQuery.WhereIf(
                !query.QuestionTypes.IsNullOrEmpty(),
                questionBank => query.QuestionTypes.Contains(questionBank.QuestionType));

            questionBankQuery = questionBankQuery.OrderByDescending(question => question.ChangedDate);

            var totalCount = await questionBankQuery.CountAsync(cancellationToken);
            questionBankQuery = ApplyPaging(questionBankQuery, query.PagedInfo);

            var entities = await questionBankQuery.Select(p => new QuestionBankModel(p)).ToListAsync(cancellationToken);

            var questionGroupId = entities
               .Where(questionBank => questionBank.QuestionGroupId.HasValue)
               .Select(questionBank => questionBank.QuestionGroupId);

            var questionGroupsById = await _questionGroupRepository
               .GetAll()
               .Where(questionGroup => questionGroupId.Contains(questionGroup.Id))
               .ToDictionaryAsync(questionGroup => questionGroup.Id, questionGroup => questionGroup.Name);

            entities = entities.Select(questionBank =>
            {
               questionBank.QuestionGroupName = questionBank.QuestionGroupId.HasValue ?
                questionGroupsById[questionBank.QuestionGroupId.Value] : string.Empty;
               return questionBank;
            }).ToList();
            return new PagedResultDto<QuestionBankModel>(totalCount, entities);
        }
    }
}
