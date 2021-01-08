using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class GetFormByIdQueryHandler : BaseQueryHandler<GetFormByIdQuery, StandaloneSurveyModel>
    {
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly ICslAccessControlContext _cslAccessControlContext;

        public GetFormByIdQueryHandler(
            IRepository<AccessRight> accessRightRepository,
            IAccessControlContext accessControlContext,
            ICslAccessControlContext cslAccessControlContext,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository) : base(accessControlContext)
        {
            _formRepository = formRepository;
            _accessRightRepository = accessRightRepository;
            _cslAccessControlContext = cslAccessControlContext;
        }

        protected override async Task<StandaloneSurveyModel> HandleAsync(GetFormByIdQuery query, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAll();

            if (query.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        SurveyEntityExpressions.HasPermissionToSeeFormExpr(CurrentUserId))
                    .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId);
            }
            else
            {
                formQuery = formQuery
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllViewableCslRoles(),
                        communityId: query.CommunityId,
                        includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr());
            }

            var form = await formQuery.FirstOrDefaultAsync(p => p.Id == query.FormId, cancellationToken);

            if (form is null)
            {
                throw new SurveyAccessDeniedException();
            }

            return new StandaloneSurveyModel(form);
        }
    }
}
