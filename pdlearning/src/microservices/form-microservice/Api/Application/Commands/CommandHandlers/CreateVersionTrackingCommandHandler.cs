using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Events;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Services;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.Services;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Application.RequestDtos;
using Microservice.Form.Versioning.Application.Services;
using Microservice.Form.Versioning.Entities;
using Microservice.Form.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Commands
{
    public class CreateVersionTrackingCommandHandler : BaseCommandHandler<CreateVersionTrackingCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IFormBusinessLogicService _formBusinessLogicService;
        private readonly FormNotifyApplicationService _formParticipantNotifyApplicationService;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly IFormUrlExtractor _formUrlExtractor;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormSection> _formSectionRepository;

        public CreateVersionTrackingCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<FormParticipant> formParticipantRepository,
            IRepository<UserEntity> userRepository,
            IRepository<AccessRight> accessRightRepository,
            IFormBusinessLogicService formBusinessLogicService,
            IAccessControlContext accessControlContext,
            FormNotifyApplicationService formParticipantNotifyApplicationService,
            VersionTrackingApplicationService versionTrackingApplicationService,
            WebAppLinkBuilder webAppLinkBuilder,
            IFormUrlExtractor formUrlExtractor,
            IRepository<FormSection> formSectionRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formParticipantRepository = formParticipantRepository;
            _formBusinessLogicService = formBusinessLogicService;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _formUrlExtractor = formUrlExtractor;
            _accessRightRepository = accessRightRepository;
            _formSectionRepository = formSectionRepository;
            _formParticipantNotifyApplicationService = formParticipantNotifyApplicationService;
            _userRepository = userRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        protected override async Task HandleAsync(CreateVersionTrackingCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasOwnerOrApprovalPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            string actionComment = "Edited";
            Guid revertObjectId = Guid.Empty;
            bool canRollback = false;
            bool increaseMajorVersion = false;
            bool increaseMinorVersion = true;

            switch (command.FormStatus)
            {
                case FormStatus.Published:
                    canRollback = true;
                    increaseMajorVersion = true;
                    actionComment = "Published";
                    revertObjectId = command.FormId;
                    break;
                case FormStatus.Unpublished:
                    actionComment = "Unpublished";
                    increaseMinorVersion = false;
                    break;
                case FormStatus.Approved:
                    actionComment = "Approved";
                    increaseMinorVersion = false;
                    break;
                case FormStatus.Rejected:
                    actionComment = "Rejected";
                    increaseMinorVersion = false;
                    break;
                case FormStatus.PendingApproval:
                    actionComment = "Submited for approval";
                    increaseMinorVersion = false;
                    break;
                case FormStatus.Draft:
                    actionComment = "Edited";
                    break;
                case FormStatus.ReadyToUse:
                    actionComment = "Readied for use";
                    increaseMinorVersion = false;
                    break;
                case FormStatus.Archived:
                    increaseMajorVersion = false;
                    increaseMinorVersion = true;
                    actionComment = "Archived";
                    break;
                default:
                    actionComment = $"Status changed to {command.FormStatus}";
                    break;
            }

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.Form,
                ObjectId = command.FormId,
                UserId = command.UserId,
                ActionComment = actionComment,
                RevertObjectId = revertObjectId,
                CanRollback = canRollback,
                IncreaseMajorVersion = increaseMajorVersion,
                IncreaseMinorVersion = increaseMinorVersion,
            });
        }
    }
}
