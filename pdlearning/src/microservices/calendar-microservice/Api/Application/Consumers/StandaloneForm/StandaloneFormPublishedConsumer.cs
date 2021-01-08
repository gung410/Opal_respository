using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers
{
    [OpalConsumer("microservice.events.form.standalone-form.published")]
    public class StandaloneFormPublishedConsumer : InboxSupportConsumer<StandaloneFormPublishedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly ILogger<StandaloneFormPublishedConsumer> _logger;

        public StandaloneFormPublishedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<PersonalEvent> personalEventRepository,
            ILogger<StandaloneFormPublishedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(StandaloneFormPublishedMessage message)
        {
            ValidateMessage(message);

            if (!message.Form.StartDate.HasValue || !message.Form.EndDate.HasValue)
            {
                _logger.LogWarning("[StandaloneFormPublishedConsumer] Validity period is not valid.");
                return;
            }

            var existedEvent = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.SourceId == message.Form.OriginalObjectId && x.Source == CalendarEventSource.StandaloneForm);
            if (existedEvent != null)
            {
                _logger.LogWarning("[StandaloneFormPublishedConsumer] Standalone Form event with SourceId {SourceId} was existed.", message.Form.OriginalObjectId);
                return;
            }

            var createEventCommand = new CreatePersonalEventCommand
            {
                UserId = message.Form.OwnerId,
                CreatedBy = message.Form.CreatedBy,
                Source = CalendarEventSource.StandaloneForm,
                SourceId = message.Form.OriginalObjectId,
                CreationRequest = new CreatePersonalEventRequest
                {
                    Title = $"{message.Form.Title} - {message.Form.Type}",
                    Description = message.FormUrl,
                    StartAt = message.Form.StartDate.Value,
                    EndAt = message.Form.EndDate.Value,
                    IsAllDay = true,
                    AttendeeIds = message.ParticipantIds
                }
            };

            await _thunderCqrs.SendCommand(createEventCommand);
        }

        private void ValidateMessage(StandaloneFormPublishedMessage message)
        {
            Guard.NotNull(message, "message");
            Guard.NotNull(message.Form.OriginalObjectId, "message.Form.OriginalObjectId");
        }
    }
}
