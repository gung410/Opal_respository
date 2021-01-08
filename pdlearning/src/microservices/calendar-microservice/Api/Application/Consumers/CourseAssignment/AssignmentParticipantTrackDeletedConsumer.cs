using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.CourseAssignment
{
    [OpalConsumer("microservice.events.course.participantassignmenttrack.deleted")]
    public class AssignmentParticipantTrackDeletedConsumer : InboxSupportConsumer<AssignmentParticipantTrackDeletedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly ILogger<AssignmentParticipantTrackDeletedConsumer> _logger;

        public AssignmentParticipantTrackDeletedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<PersonalEvent> personalEventRepository,
            ILogger<AssignmentParticipantTrackDeletedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(AssignmentParticipantTrackDeletedMessage message)
        {
            var existedEvent = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseAssignment && x.SourceId == message.Id);

            if (existedEvent == null)
            {
                _logger.LogWarning("[AssignmentParticipantTrackDeletedConsumer] Event with Id {0} was not existed.", message.Id);
                return;
            }

            var deletePersonalEvent = new DeletePersonalEventCommand
            {
                EventId = existedEvent.Id
            };

            await _thunderCqrs.SendCommand(deletePersonalEvent);
        }
    }
}
