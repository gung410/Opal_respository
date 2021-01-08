using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microservice.StandaloneSurvey.Infrastructure;
using Microservice.StandaloneSurvey.Versioning.Application.RequestDtos;
using Microservice.StandaloneSurvey.Versioning.Application.Services;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Microservice.StandaloneSurvey.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands.CommandHandlers
{
    public class ArchiveFormCommandHandler : BaseCommandHandler<ArchiveSurveyCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly IRepository<SurveySection> _formSectionRepository;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly ICslAccessControlContext _cslAccessControlContext;

        public ArchiveFormCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<AccessRight> accessRightRepository,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveyQuestion> formQuestionRepository,
            IRepository<SurveySection> formSectionRepository,
            VersionTrackingApplicationService versionTrackingApplicationService,
            IUnitOfWorkManager unitOfWorkManager,
            ICslAccessControlContext cslAccessControlContext,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formSectionRepository = formSectionRepository;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _cslAccessControlContext = cslAccessControlContext;
        }

        protected override async Task HandleAsync(ArchiveSurveyCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAll();

            if (command.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        SurveyEntityExpressions.HasOwnerPermissionExpr(this.CurrentUserId));
            }
            else
            {
                formQuery = formQuery
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllManageableCslRoles(),
                        communityId: command.CommunityId,
                        includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr());
            }

            var originalForm = await formQuery.FirstOrDefaultAsync(f => f.Id == command.FormId, cancellationToken);

            if (originalForm == null)
            {
                throw new SurveyAccessDeniedException();
            }

            originalForm.ArchivedBy = command.ArchiveBy ?? Guid.Empty;
            originalForm.ArchiveDate = Clock.Now;
            originalForm.Status = SurveyStatus.Archived;

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.StandaloneSurvey,
                ObjectId = originalForm.Id,
                UserId = CurrentUserId,
                ActionComment = "Archived",
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = true,
            });

            await _formRepository.UpdateAsync(originalForm);
        }
    }
}
