using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.CourseAssignment
{
    [OpalConsumer("microservice.events.course.assignment.deleted")]
    public class CourseAssignmentDeletedConsumer : InboxSupportConsumer<CourseAssignmentDeletedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public CourseAssignmentDeletedConsumer(IThunderCqrs thunderCqrs, IRepository<PersonalEvent> personalEventRepository)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
        }

        public async Task InternalHandleAsync(CourseAssignmentDeletedMessage message)
        {
            var relatedAssignmentEvents = await _personalEventRepository
                .GetAll()
                .Where(x => x.Source == CalendarEventSource.CourseAssignment && (x.SourceId == message.Id && x.SourceParentId == message.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (relatedAssignmentEvents.Count > 0)
            {
                await _thunderCqrs.SendCommand(new DeleteCourseAssignmentEventByIdsCommand { EventIds = relatedAssignmentEvents });
            }
        }
    }
}
