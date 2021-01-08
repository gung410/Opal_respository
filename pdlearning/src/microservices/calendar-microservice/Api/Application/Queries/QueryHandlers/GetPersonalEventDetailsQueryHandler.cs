using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microservice.Calendar.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries.QueryHandlers
{
    public class GetPersonalEventDetailsQueryHandler : BaseThunderQueryHandler<GetPersonalEventDetailsByIdQuery, PersonalEventDetailsModel>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;

        public GetPersonalEventDetailsQueryHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
        }

        protected override async Task<PersonalEventDetailsModel> HandleAsync(GetPersonalEventDetailsByIdQuery query, CancellationToken cancellationToken)
        {
            var eventSelected = await _personalEventRepository.FirstOrDefaultAsync(e => e.Id == query.EventId);

            if (eventSelected == null)
            {
                throw new EntityNotFoundException(typeof(PersonalEvent), query.EventId);
            }

            var rootEventId = query.EventId;
            if (eventSelected.Source == CalendarEventSource.CourseSession)
            {
                var classRunEventQuery = await _personalEventRepository
                    .GetAll()
                    .Where(e => e.Source == CalendarEventSource.CourseClassRun && e.SourceId == eventSelected.SourceParentId.Value)
                    .FirstOrDefaultAsync();

                if (classRunEventQuery == null)
                {
                    throw new EntityNotFoundException(typeof(PersonalEvent), query.EventId);
                }

                rootEventId = classRunEventQuery.Id;
            }

            var attendeeIds = await _userEventRepository
                .GetAll()
                .GetAcceptedEvents()
                .Where(x => x.EventId == rootEventId)
                .Select(p => p.UserId)
                .ToListAsync();

            if (!attendeeIds.Contains(query.UserId))
            {
                throw new EntityNotFoundException(typeof(PersonalEvent), query.EventId);
            }

            var eventDetail = await _personalEventRepository
                .GetAll()
                .Where(x => x.Id == query.EventId)
                .Select(e => new PersonalEventDetailsModel()
                {
                    AttendeeIds = attendeeIds,
                    CreatedBy = e.CreatedBy,
                    CreatedAt = e.CreatedDate,
                    Description = e.Description,
                    EndAt = e.EndAt,
                    Id = e.Id,
                    IsAllDay = e.IsAllDay,
                    Source = e.Source,
                    SourceId = e.SourceId,
                    StartAt = e.StartAt,
                    Title = e.Title,
                    Type = e.Type,
                    RepeatFrequency = e.RepeatFrequency,
                    RepeatUntil = e.RepeatUntil
                }).FirstOrDefaultAsync(cancellationToken);
            return eventDetail;
        }
    }
}
