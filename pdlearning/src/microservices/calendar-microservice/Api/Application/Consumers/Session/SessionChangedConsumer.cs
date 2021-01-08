using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Application.Consumers.Messages.Helpers;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.Session
{
    [OpalConsumer("microservice.events.course.session.created")]
    [OpalConsumer("microservice.events.course.session.updated")]
    public class SessionChangedConsumer : InboxSupportConsumer<SessionChangedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public SessionChangedConsumer(IThunderCqrs thunderCqrs, IRepository<PersonalEvent> personalEventRepository)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
        }

        public async Task InternalHandleAsync(SessionChangedMessage message)
        {
            var existedSession = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseSession && x.SourceId == message.Id && x.SourceParentId == message.ClassRunId);

            if (existedSession == null)
            {
                await _thunderCqrs.SendCommand(BuildCreateSessionCommand(message));
                return;
            }

            await _thunderCqrs.SendCommand(BuildUpdateSessionCommand(message));
        }

        private CreateSessionEventCommand BuildCreateSessionCommand(SessionChangedMessage message)
        {
            return new CreateSessionEventCommand
            {
                SessionId = message.Id,
                ClassRunId = message.ClassRunId,
                Title = message.SessionTitle,
                StartDateTime = message.StartDateTime,
                EndDateTime = message.EndDateTime,
                Status = ClassRunStatusMapper.GetClassRunEventStatus(message.ClassRun.Status, message.Course.Status)
            };
        }

        private UpdateSessionEventCommand BuildUpdateSessionCommand(SessionChangedMessage message)
        {
            return new UpdateSessionEventCommand
            {
                SessionId = message.Id,
                ClassRunId = message.ClassRunId,
                Title = message.SessionTitle,
                StartDateTime = message.StartDateTime,
                EndDateTime = message.EndDateTime,
                Status = ClassRunStatusMapper.GetClassRunEventStatus(message.ClassRun.Status, message.Course.Status)
            };
        }
    }
}
