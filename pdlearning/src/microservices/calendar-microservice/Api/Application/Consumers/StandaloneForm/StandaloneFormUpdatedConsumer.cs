using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers
{
    [OpalConsumer("microservice.events.form.updated")]
    public class StandaloneFormUpdatedConsumer : InboxSupportConsumer<StandaloneFormUpdatedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly ILogger<StandaloneFormUpdatedConsumer> _logger;

        public StandaloneFormUpdatedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<PersonalEvent> personalEventRepository,
            ILogger<StandaloneFormUpdatedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(StandaloneFormUpdatedMessage message)
        {
            ValidateMessage(message);
            if (message.Status != FormStatus.Unpublished)
            {
                return;
            }

            var existedEvent = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.SourceId == message.OriginalObjectId && x.Source == CalendarEventSource.StandaloneForm);
            if (existedEvent == null)
            {
                _logger.LogWarning("[StandaloneFormUpdatedConsumer] Standalone Form event with SourceId {SourceId} was not existed.", message.OriginalObjectId);
                return;
            }

            var deleteEventCommand = new DeletePersonalEventCommand
            {
                EventId = existedEvent.Id
            };
            await _thunderCqrs.SendCommand(deleteEventCommand);
        }

        private void ValidateMessage(StandaloneFormUpdatedMessage message)
        {
            Guard.NotNull(message, "message");
            Guard.NotNull(message.Status, "message.Status");
            Guard.NotNull(message.OriginalObjectId, "message.OriginalObjectId");
        }
    }
}
