using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Extensions;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class CreatePersonalEventCommandHandler : BaseCommandHandler<CreatePersonalEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;
        private readonly IRepository<CalendarUser> _userRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public CreatePersonalEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IRepository<CalendarUser> userRepository,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
            _userRepository = userRepository;
            this._thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(CreatePersonalEventCommand command, CancellationToken cancellationToken)
        {
            var newEvent = new PersonalEvent(command.CreationRequest.Id)
                .WithBasicInfo(command.CreationRequest.Title, command.CreationRequest.Description)
                .WithTime(
                    startAt: command.CreationRequest.StartAt,
                    endAt: command.CreationRequest.EndAt,
                    repeatUntil: command.CreationRequest.RepeatUntil,
                    repeatFrequency: command.CreationRequest.RepeatFrequency,
                    isAllDay: command.CreationRequest.IsAllDay)
                .FromSource(command.Source, command.SourceId)
                .WithOwner(command.CreatedBy);

            await _personalEventRepository.InsertAsync(newEvent);

            // Save attendees to user event, includes owner.
            var attendeeIds = command.CreationRequest.AttendeeIds ?? new List<Guid>();
            attendeeIds.Add(command.UserId.Value);
            attendeeIds = attendeeIds.Distinct().ToList();

            await CheckAttendeesExist(attendeeIds);

            var userEvents = attendeeIds
                .Distinct()
                .Select(p => new UserPersonalEvent { EventId = newEvent.Id, UserId = p })
                .ToList();
            await _userEventRepository.InsertManyAsync(userEvents);
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
