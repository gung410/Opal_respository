using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Application.Events.EventPayloads;
using Microservice.StandaloneSurvey.Application.Events;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands.CommandHandlers
{
    public class TransferOwnershipCommandHandler : BaseCommandHandler<TransferOwnershipCommand>
    {
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SyncedUser> _userRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public TransferOwnershipCommandHandler(
            WebAppLinkBuilder webAppLinkBuilder,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SyncedUser> userRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs) : base(unitOfWorkManager, accessControlContext)
        {
            _webAppLinkBuilder = webAppLinkBuilder;
            _formRepository = formRepository;
            _userRepository = userRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(TransferOwnershipCommand command, CancellationToken cancellationToken)
        {
            if (command.SubModule == SubModule.Csl)
            {
                throw new NotSupportedFeatureException();
            }

            var dbQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(AccessControlContext, SurveyEntityExpressions.HasOwnerPermissionExpr(this.CurrentUserId));

            var existedForm = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.Request.ObjectId, cancellationToken);

            if (existedForm == null)
            {
                throw new SurveyAccessDeniedException();
            }

            var oldOnwerId = existedForm.OwnerId;
            existedForm.OwnerId = command.Request.NewOwnerId;
            await _formRepository.UpdateAsync(existedForm);

            var formUrl = _webAppLinkBuilder.GetFormDetailLink(existedForm.Id, command.SubModule);
            var ownerName = _userRepository.FirstOrDefault(x => x.Id == oldOnwerId).FullName();
            var newOwnerName = _userRepository.FirstOrDefault(x => x.Id == command.Request.NewOwnerId).FullName();

            var payload = new NotifyTransferOwnershipPayload
            {
                ObjectName = existedForm.Title,
                ObjectDetailUrl = formUrl,
                OwnerName = ownerName,
                NewOwnerName = newOwnerName,
                ActionUrl = formUrl,
            };

            var reminderByConditions = new ReminderByDto
            {
                Type = ReminderByType.AbsoluteDateTimeUTC,
                Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
            };

            await _thunderCqrs.SendEvent(
                new FormNotifyTransferOwnershipEvent(
                    payload,
                    reminderByConditions,
                    existedForm.Id,
                    command.Request.NewOwnerId));
        }
    }
}
