using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Infrastructure;
using Microservice.StandaloneSurvey.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class SearchFormQuestionsQueryHandler : BaseQueryHandler<SearchFormQuestionsQuery, PagedResultDto<SurveyQuestionModel>>
    {
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly ICslAccessControlContext _cslAccessControlContext;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;

        public SearchFormQuestionsQueryHandler(
            IRepository<SurveyQuestion> formQuestionRepository,
            IAccessControlContext accessControlContext,
            ICslAccessControlContext cslAccessControlContext,
            IRepository<AccessRight> accessRightRepository,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository) : base(accessControlContext)
        {
            _formQuestionRepository = formQuestionRepository;
            _cslAccessControlContext = cslAccessControlContext;
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepository;
        }

        protected override async Task<PagedResultDto<SurveyQuestionModel>> HandleAsync(SearchFormQuestionsQuery query, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAll();

            if (query.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        SurveyEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId))
                    .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                    .IgnoreArchivedItems();
            }
            else
            {
                formQuery = formQuery
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllViewableCslRoles(),
                        communityId: query.CommunityId,
                        includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr())
                    .IgnoreArchivedItems();
            }

            var canUserAccessForm = await formQuery.AnyAsync(p => p.Id == query.FormId, cancellationToken);

            if (!canUserAccessForm)
            {
                throw new SurveyAccessDeniedException();
            }

            var formQuestionQuery = _formQuestionRepository.GetAll().Where(p => p.SurveyId == query.FormId);
            var totalCount = await formQuestionQuery.CountAsync(cancellationToken);

            formQuestionQuery = ApplyPaging(formQuestionQuery, query.PagedInfo);

            var entities = await formQuestionQuery
                .OrderBy(question => question.Priority)
                .ThenBy(question => question.MinorPriority)
                .Select(question => new SurveyQuestionModel(question))
                .ToListAsync(cancellationToken);

            return new PagedResultDto<SurveyQuestionModel>(totalCount, entities);
        }
    }
}
