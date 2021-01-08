using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Events;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Application.RequestDtos;
using Microservice.Form.Versioning.Application.Services;
using Microservice.Form.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Commands.CommandHandlers
{
    public class ArchiveFormCommandHandler : BaseCommandHandler<ArchiveFormCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormSection> _formSectionRepository;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;

        public ArchiveFormCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<AccessRight> accessRightRepository,
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<FormSection> formSectionRepository,
            VersionTrackingApplicationService versionTrackingApplicationService,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formSectionRepository = formSectionRepository;
            _versionTrackingApplicationService = versionTrackingApplicationService;
        }

        protected override async Task HandleAsync(ArchiveFormCommand command, CancellationToken cancellationToken)
        {
            var dbQuery = _formRepository
                .GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasOwnerPermissionExpr(this.CurrentUserId));

            var originalForm = await dbQuery.FirstOrDefaultAsync(f => f.Id == command.FormId);

            if (originalForm == null)
            {
                throw new FormAccessDeniedException();
            }

            originalForm.ArchivedBy = command.ArchiveBy ?? Guid.Empty;
            originalForm.ArchiveDate = Clock.Now;
            originalForm.Status = FormStatus.Archived;

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.Form,
                ObjectId = originalForm.Id,
                UserId = CurrentUserId,
                ActionComment = "Archived",
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = true,
            });

            await _formRepository.UpdateAsync(originalForm);

            await _thunderCqrs.SendEvent(new FormChangeEvent(originalForm, FormChangeType.Archived), cancellationToken);
        }
    }
}
