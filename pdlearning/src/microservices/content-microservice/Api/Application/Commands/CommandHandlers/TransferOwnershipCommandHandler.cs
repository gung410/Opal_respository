using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Application.Events.EventPayloads;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Extensions;
using Microservice.Form.Application.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class TransferOwnershipCommandHandler : BaseCommandHandler<TransferOwnershipCommand>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IConfiguration _configuration;

        public TransferOwnershipCommandHandler(
            IRepository<DigitalContent> digitalContentRepository,
            IRepository<UserEntity> userRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs,
            IConfiguration configuration) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _userRepository = userRepository;
            _thunderCqrs = thunderCqrs;
            _configuration = configuration;
        }

        protected override async Task HandleAsync(TransferOwnershipCommand command, CancellationToken cancellationToken)
        {
            var dbQuery = _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .IgnoreArchivedItems();

            var digitalContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.Request.ObjectId, cancellationToken);

            if (digitalContent == null)
            {
                throw new ContentAccessDeniedException();
            }

            var oldOnwerId = digitalContent.OwnerId;
            digitalContent.OwnerId = command.Request.NewOwnerId;
            await _digitalContentRepository.UpdateAsync(digitalContent);

            var ownerName = _userRepository.FirstOrDefault(x => x.Id == oldOnwerId).FullName();
            var contentUrl = WebAppLinkBuilder.GetDigitalContentDetailLink(_configuration, digitalContent.Id);
            var newOwnerName = _userRepository.FirstOrDefault(x => x.Id == command.Request.NewOwnerId).FullName();

            var payload = new NotifyTransferOwnershipPayload
            {
                ObjectName = digitalContent.Title,
                ObjectDetailUrl = contentUrl,
                OwnerName = ownerName,
                NewOwnerName = newOwnerName,
                ActionUrl = contentUrl,
            };

            var reminderByConditions = new ReminderByDto
            {
                Type = ReminderByType.AbsoluteDateTimeUTC,
                Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
            };

            await _thunderCqrs.SendEvent(
               new NotifyContentTransferOwnershipEvent(
                   payload,
                   reminderByConditions,
                   digitalContent.Id,
                   command.Request.NewOwnerId));
        }
    }
}
