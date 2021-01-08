using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Events;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Infrastructure;
using Microservice.StandaloneSurvey.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class MarkFormAsArchivedCommandHandler : BaseCommandHandler<MarkFormAsArchivedCommand>
    {
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly ICslAccessControlContext _cslAccessControlContext;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public MarkFormAsArchivedCommandHandler(
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            ICslAccessControlContext cslAccessControlContext,
            IAccessControlContext accessControlContext,
            IRepository<AccessRight> accessRightRepository,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formRepository = formRepository;
            _cslAccessControlContext = cslAccessControlContext;
            _accessRightRepository = accessRightRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(MarkFormAsArchivedCommand command, CancellationToken cancellationToken)
        {
            await Update(command, cancellationToken);
        }

        private async Task Update(MarkFormAsArchivedCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAll();

            if (command.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        SurveyEntityExpressions.HasOwnerPermissionExpr(this.CurrentUserId))
                    .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                    .IgnoreArchivedItems();
            }
            else
            {
                formQuery = formQuery
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllManageableCslRoles(),
                        communityId: command.CommunityId,
                        includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr())
                    .IgnoreArchivedItems();
            }

            var existedForm = await formQuery.FirstOrDefaultAsync(dc => dc.Id == command.Id, cancellationToken);

            if (existedForm == null)
            {
                throw new SurveyAccessDeniedException();
            }

            existedForm.IsArchived = true;
            await _formRepository.UpdateAsync(existedForm);

            await _thunderCqrs.SendEvent(new SurveyChangeEvent(existedForm, SurveyChangeType.Archived), cancellationToken);
        }
    }
}
