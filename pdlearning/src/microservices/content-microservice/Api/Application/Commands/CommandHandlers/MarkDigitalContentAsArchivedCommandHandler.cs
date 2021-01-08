using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Common.Extensions;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class MarkDigitalContentAsArchivedCommandHandler : BaseCommandHandler<MarkDigitalContentAsArchivedCommand>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IOutboxQueue _outboxQueue;

        public MarkDigitalContentAsArchivedCommandHandler(
            IRepository<DigitalContent> digitalContentRepository,
            IRepository<AccessRight> accessRightRepository,
            IAccessControlContext accessControlContext,
            IThunderCqrs thunderCqrs,
            IOutboxQueue outboxQueue,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _accessRightRepository = accessRightRepository;
            _digitalContentRepository = digitalContentRepository;
            _thunderCqrs = thunderCqrs;
            _outboxQueue = outboxQueue;
        }

        protected override async Task HandleAsync(MarkDigitalContentAsArchivedCommand command, CancellationToken cancellationToken)
        {
            var dbQuery = _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_digitalContentRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            var existedDigitalContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.Id, cancellationToken);

            if (existedDigitalContent == null)
            {
                throw new ContentAccessDeniedException();
            }

            existedDigitalContent.IsArchived = true;
            await _digitalContentRepository.UpdateAsync(existedDigitalContent);

            var model = new DigitalContentModel(existedDigitalContent);

            await _outboxQueue.QueueMessageAsync(DigitalContentChangeType.Archived, model, UserContext);

            await RemoveExpiredEmailEvents(existedDigitalContent, cancellationToken);
        }

        private async Task RemoveExpiredEmailEvents(DigitalContent existedDigitalContent, CancellationToken cancellationToken)
        {
            await _thunderCqrs.SendEvent(
               new PreNotifyContentCompletedEvent(existedDigitalContent.Id, Status.Complete), cancellationToken);

            await _thunderCqrs.SendEvent(
               new NotifyContentCompletedEvent(existedDigitalContent.Id, Status.Complete), cancellationToken);
        }
    }
}
