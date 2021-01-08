using System;
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

namespace Microservice.Calendar.Application.Consumers.Session
{
    [OpalConsumer("microservice.events.course.session.deleted")]
    public class SessionDeletedConsumer : InboxSupportConsumer<SessionDeletedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly ILogger<SessionDeletedConsumer> _logger;

        public SessionDeletedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<PersonalEvent> personalEventRepository,
            ILogger<SessionDeletedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(SessionDeletedMessage message)
        {
            var existedSession = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseSession && x.SourceId == message.Id && x.SourceParentId == message.ClassRunId);

            if (existedSession == null)
            {
                _logger.LogWarning("[SessionDeletedConsumer] Session with Id {SessionId} was not existed.", message.Id);
                return;
            }

            var deleteSessionCommand = new DeleteSessionEventCommand
            {
                SessionId = message.Id,
                ClassRunId = message.ClassRunId
            };

            await _thunderCqrs.SendCommand(deleteSessionCommand);
        }
    }
}
