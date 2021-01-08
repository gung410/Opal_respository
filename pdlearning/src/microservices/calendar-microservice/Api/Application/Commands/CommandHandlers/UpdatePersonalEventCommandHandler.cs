using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Exceptions;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class UpdatePersonalEventCommandHandler : BaseCommandHandler<UpdatePersonalEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;
        private readonly IRepository<CalendarUser> _userRepository;

        public UpdatePersonalEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IRepository<CalendarUser> userRepository,
            IUnitOfWorkManager unitOfWork,
            IUserContext userContext)
            : base(unitOfWork, userContext)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
            _userRepository = userRepository;
        }

        protected override async Task HandleAsync(UpdatePersonalEventCommand command, CancellationToken cancellationToken)
        {
            await CheckAttendeesExist(command.Request.AttendeeIds);

            var eventEntity = await _personalEventRepository.GetAsync(command.Request.Id);
            if (eventEntity == null)
            {
                throw new EntityNotFoundException(typeof(PersonalEvent), command.Request.Id);
            }

            if (eventEntity.Source != CalendarEventSource.SelfCreated || (command.UserId.HasValue && command.UserId.Value != eventEntity.CreatedBy))
            {
                throw new UpdateEventDeniedException();
            }

            eventEntity.Description = command.Request.Description;
            eventEntity.EndAt = command.Request.EndAt;
            eventEntity.IsAllDay = command.Request.IsAllDay;
            eventEntity.StartAt = command.Request.StartAt;
            eventEntity.Title = command.Request.Title;
            eventEntity.RepeatUntil = command.Request.RepeatUntil;
            eventEntity.RepeatFrequency = command.Request.RepeatFrequency;
            await _personalEventRepository.UpdateAsync(eventEntity);

            await UpdateUserEvents(eventEntity, command.Request.AttendeeIds);
        }

        private async Task UpdateUserEvents(PersonalEvent eventEntity, List<Guid> requestAttendeeIds)
        {
            requestAttendeeIds ??= new List<Guid>();
            if (eventEntity.CreatedBy.HasValue)
            {
                requestAttendeeIds.Remove(eventEntity.CreatedBy.Value);
            }

            var userEvents = await _userEventRepository.GetAllListAsync(userEvent => userEvent.EventId == eventEntity.Id);
            var owner = userEvents.FirstOrDefault(ue => ue.UserId == eventEntity.CreatedBy);
            if (owner != null)
            {
                userEvents.Remove(owner);
            }

            var toBeDeletedAttendees = userEvents.Where(ue => !requestAttendeeIds.Contains(ue.UserId)).ToList();
            if (toBeDeletedAttendees.Any())
            {
                await _userEventRepository.DeleteManyAsync(toBeDeletedAttendees);
            }

            var attendeeIds = userEvents.Select(ue => ue.UserId).ToList();
            var newAttendees = requestAttendeeIds.Except(attendeeIds).Select(id => new UserPersonalEvent()
            {
                EventId = eventEntity.Id,
                Id = Guid.NewGuid(),
                UserId = id
            }).ToList();
            if (newAttendees.Any())
            {
                await _userEventRepository.InsertManyAsync(newAttendees);
            }
        }

        private async Task CheckAttendeesExist(List<Guid> attendeeIds)
        {
            if (attendeeIds == null || !attendeeIds.Any())
            {
                return;
            }

            var users = await _userRepository.GetAllListAsync(u => attendeeIds.Contains(u.Id));
            var notExistedAttendeeIds = attendeeIds.Where(id => !users.Any(u => u.Id == id)).ToList();

            if (notExistedAttendeeIds.Any())
            {
                throw new EntityNotFoundException(typeof(CalendarUser), notExistedAttendeeIds.First());
            }
        }
    }
}
