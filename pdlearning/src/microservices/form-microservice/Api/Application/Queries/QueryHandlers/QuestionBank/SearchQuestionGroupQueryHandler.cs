using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Form.Application.Queries
{
    public class SearchQuestionGroupQueryHandler : BaseQueryHandler<SearchQuestionGroupQuery, PagedResultDto<QuestionGroupModel>>
    {
        private readonly IRepository<QuestionGroup> _questionGroupRepository;
        private readonly IRepository<QuestionBank> _questionBankRepository;

        public SearchQuestionGroupQueryHandler(
            IAccessControlContext accessControlContext,
            IRepository<QuestionGroup> questionGroupRepository,
            IRepository<QuestionBank> questionBankRepository) : base(accessControlContext)
        {
            _questionGroupRepository = questionGroupRepository;
            _questionBankRepository = questionBankRepository;
        }

        protected override async Task<PagedResultDto<QuestionGroupModel>> HandleAsync(SearchQuestionGroupQuery query, CancellationToken cancellationToken)
        {
            var questionGroupQuery = _questionGroupRepository
                .GetAll()
                .WhereIf(
                    !string.IsNullOrEmpty(query.Name),
                    questionGroup => EF.Functions.Contains(questionGroup.Name, "\"" + query.Name + "*\""));

            if (query.IsFilterByUsing)
            {
                var groupsIdInUsing = await _questionBankRepository
                    .GetAll()
                    .Where(question => question.CreatedBy == CurrentUserId)
                    .Select(question => question.QuestionGroupId)
                    .ToListAsync(cancellationToken);

                if (groupsIdInUsing.Any())
                {
                    questionGroupQuery = questionGroupQuery.Where(questionGroup => groupsIdInUsing.Contains(questionGroup.Id));
                }
            }

            var totalCount = await questionGroupQuery.CountAsync(cancellationToken);
            questionGroupQuery = ApplyPaging(questionGroupQuery, query.PagedInfo);
            var entities = await questionGroupQuery.Select(p => new QuestionGroupModel(p)).ToListAsync(cancellationToken);

            return new PagedResultDto<QuestionGroupModel>(totalCount, entities);
        }
    }
}
