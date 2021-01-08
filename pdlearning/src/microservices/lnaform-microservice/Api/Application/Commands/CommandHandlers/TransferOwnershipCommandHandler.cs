using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Application.Events.EventPayloads;
using Microservice.LnaForm.Application.Events;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Commands.CommandHandlers
{
    public class TransferOwnershipCommandHandler : BaseCommandHandler<TransferOwnershipCommand>
    {
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public TransferOwnershipCommandHandler(
            WebAppLinkBuilder webAppLinkBuilder,
            IRepository<FormEntity> formRepository,
            IRepository<UserEntity> userRepository,
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
            var dbQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(AccessControlContext, LnaFormEntityExpressions.HasOwnerPermissionExpr(this.CurrentUserId));

            var existedForm = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.Request.ObjectId, cancellationToken);

            if (existedForm == null)
            {
                throw new FormAccessDeniedException();
            }

            var oldOnwerId = existedForm.OwnerId;
            existedForm.OwnerId = command.Request.NewOwnerId;
            await _formRepository.UpdateAsync(existedForm);

            var formUrl = _webAppLinkBuilder.GetFormDetailLink(existedForm.Id);
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
