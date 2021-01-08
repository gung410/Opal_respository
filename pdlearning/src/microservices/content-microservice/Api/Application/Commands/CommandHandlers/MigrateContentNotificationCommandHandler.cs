using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Infrastructure;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class MigrateContentNotificationCommandHandler : BaseCommandHandler<MigrateContentNotificationCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<DigitalContent> _contentRepository;
        private readonly IOutboxQueue _outboxQueue;

        public MigrateContentNotificationCommandHandler(
           IThunderCqrs thunderCqrs,
           IUnitOfWorkManager unitOfWorkManager,
           IRepository<DigitalContent> contentRepository,
           IOutboxQueue outboxQueue,
           IUserContext userContext,
           IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _contentRepository = contentRepository;
            _outboxQueue = outboxQueue;
        }

        protected override async Task HandleAsync(MigrateContentNotificationCommand command, CancellationToken cancellationToken)
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new ContentAccessDeniedException();
            }

            var digitalContentModels = _contentRepository
                .GetAll()
                .Where(content => command.ListIds.Contains(content.Id))
                .Select(p => new DigitalContentModel(p))
                .ToList();

            await _outboxQueue.QueueMessagesAsync(DigitalContentChangeType.Updated, digitalContentModels, UserContext);
        }
    }
}
