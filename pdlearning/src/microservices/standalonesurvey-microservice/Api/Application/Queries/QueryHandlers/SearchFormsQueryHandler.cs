using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microservice.StandaloneSurvey.Infrastructure;
using Microservice.StandaloneSurvey.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class SearchFormsQueryHandler : BaseQueryHandler<SearchFormsQuery, PagedResultDto<StandaloneSurveyModel>>
    {
        private readonly ICslAccessControlContext _cslAccessControlContext;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;
        private readonly IUserContext _userContext;

        public SearchFormsQueryHandler(
            IAccessControlContext accessControlContext,
            ICslAccessControlContext cslAccessControlContext,
            IRepository<AccessRight> accessRightRepository,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveyParticipant> formParticipantRepository,
            IUserContext userContext) : base(accessControlContext)
        {
            _cslAccessControlContext = cslAccessControlContext;
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepository;
            _formParticipantRepository = formParticipantRepository;
            _userContext = userContext;
        }

        protected override async Task<PagedResultDto<StandaloneSurveyModel>> HandleAsync(SearchFormsQuery query, CancellationToken cancellationToken)
        {
            var unionExpression = SurveyEntityExpressions.HasOwnerPermissionExpr(CurrentUserId);

            if (query.IncludeFormForImportToCourse)
            {
                unionExpression = SurveyEntityExpressions.AvailableForImportToCourseExpr(CurrentUserId);
            }

            var formQuery = _formRepository.GetAll();

            if (query.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        unionExpression)
                    .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                    .IgnoreArchivedItems();
            }
            else
            {
                formQuery = FilterSurveyForCsl(formQuery, query);
            }

            formQuery = formQuery.WhereIf(
                !string.IsNullOrEmpty(query.SearchFormTitle),
                f => EF.Functions.Like(f.Title, $"%{query.SearchFormTitle}%"));

            formQuery = formQuery.WhereIf(
                !query.FilterByStatus.IsNullOrEmpty(),
                f => query.FilterByStatus.Contains(f.Status));

            if (query.FilterByStatus.IsNullOrEmpty())
            {
                formQuery = formQuery.Where(r => r.Status != SurveyStatus.Archived);
            }

            var totalCount = await formQuery.CountAsync(cancellationToken);

            formQuery = formQuery.OrderByDescending(p => p.ChangedDate != null ? p.ChangedDate.Value : p.CreatedDate);

            formQuery = ApplyPaging(formQuery, query.PagedInfo);

            var entities = await formQuery.Select(p => new StandaloneSurveyModel(p)).ToListAsync(cancellationToken);
            foreach (var form in entities)
            {
                form.CanUnpublishFormStandalone = _formParticipantRepository.Count(m => m.SurveyOriginalObjectId == form.OriginalObjectId && m.Status == SurveyParticipantStatus.Completed) == 0;
            }

            return new PagedResultDto<StandaloneSurveyModel>(totalCount, entities);
        }

        private IQueryable<Domain.Entities.StandaloneSurvey> FilterSurveyForCsl(IQueryable<Domain.Entities.StandaloneSurvey> formQuery, SearchFormsQuery query)
        {
            return query.IsOnlyCslSurveysForManagement
                ? formQuery // owners and co-owners see all surveys of themselves.
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllViewableCslRoles(),
                        communityId: query.CommunityId)
                    .IgnoreArchivedItems()
                : formQuery // All csl roles can see all published surveys to play.
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllViewableCslRoles(),
                        communityId: query.CommunityId,
                        includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr())
                    .IgnoreArchivedItems();
        }
    }
}
