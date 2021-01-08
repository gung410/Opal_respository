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
    [OpalConsumer("microservice.events.course.registration.deleted")]
    public class RegistrationDeletedConsumer : InboxSupportConsumer<RegistrationDeletedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly ILogger<RegistrationDeletedConsumer> _logger;

        public RegistrationDeletedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<UserPersonalEvent> userEventRepository,
            IRepository<PersonalEvent> personalEventRepository,
            ILogger<RegistrationDeletedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _userEventRepository = userEventRepository;
            _personalEventRepository = personalEventRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(RegistrationDeletedMessage message)
        {
            var classRunEventExisted = await _personalEventRepository
               .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseClassRun && x.SourceId == message.ClassRunId);

            if (classRunEventExisted == null)
            {
                _logger.LogWarning("[RegistrationDeletedConsumer] ClassRun with Id {ClassRunId} was not existed.", message.ClassRunId);
                return;
            }

            var userEventExisted = await _userEventRepository
                .FirstOrDefaultAsync(x => x.UserId == message.UserId && x.EventId == classRunEventExisted.Id);

            var deleteRegistrationCommand = new DeleteRegistrationCommand
            {
                ClassRunId = message.ClassRunId,
                UserId = message.UserId
            };

            await _thunderCqrs.SendCommand(deleteRegistrationCommand);
        }
    }
}
