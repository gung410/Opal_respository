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

namespace Microservice.Calendar.Application.Consumers.Registration
{
    [OpalConsumer("microservice.events.course.registration.created")]
    [OpalConsumer("microservice.events.course.registration.updated")]
    public class RegistrationChangedConsumer : InboxSupportConsumer<RegistrationChangedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly ILogger<RegistrationChangedConsumer> _logger;

        public RegistrationChangedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<UserPersonalEvent> userEventRepository,
            IRepository<PersonalEvent> personalEventRepository,
            ILogger<RegistrationChangedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _userEventRepository = userEventRepository;
            _personalEventRepository = personalEventRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(RegistrationChangedMessage message)
        {
            var classRunEventExisted = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseClassRun && x.SourceId == message.ClassRunId);

            if (classRunEventExisted == null)
            {
                _logger.LogWarning("[RegistrationChangedConsumer] ClassRun with Id {ClassRunId} was not existed.", message.ClassRunId);
                return;
            }

            var userEventExisted = await _userEventRepository
                .FirstOrDefaultAsync(x => x.UserId == message.UserId && x.EventId == classRunEventExisted.Id);

            if (userEventExisted == null)
            {
                await _thunderCqrs.SendCommand(BuildCreateRegistrationCommand(message));
                return;
            }

            await _thunderCqrs.SendCommand(BuildUpdateRegistrationCommand(message));
        }

        private CreateRegistrationCommand BuildCreateRegistrationCommand(RegistrationChangedMessage message)
        {
            return new CreateRegistrationCommand
            {
                ClassRunId = message.ClassRunId,
                UserId = message.UserId,
                IsAccepted = message.IsParticipant
            };
        }

        private UpdateRegistrationCommand BuildUpdateRegistrationCommand(RegistrationChangedMessage message)
        {
            return new UpdateRegistrationCommand
            {
                ClassRunId = message.ClassRunId,
                UserId = message.UserId,
                IsAccepted = message.IsParticipant
            };
        }
    }
}
