using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Webinar.Application.Commands.CommandHandlers
{
    public class UpdateAttendeesCommandHandler : BaseCommandHandler<UpdateAttendeesCommand>
    {
        private readonly IRepository<Attendee> _attendeeRepository;

        public UpdateAttendeesCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<Attendee> attendeeRepository) : base(unitOfWorkManager, userContext)
        {
            _attendeeRepository = attendeeRepository;
        }

        protected override async Task HandleAsync(UpdateAttendeesCommand command, CancellationToken cancellationToken)
        {
            if (!command.Attendees.Any())
            {
                await _attendeeRepository.DeleteAsync(x => x.MeetingId == command.MeetingId);
                return;
            }

            var oldAttendeeIds = await _attendeeRepository.GetAll()
                .Where(x => x.MeetingId == command.MeetingId)
                .Select(p => p.UserId)
                .ToListAsync();

            if (!oldAttendeeIds.Any())
            {
                var attendeesInsert = command.Attendees
                    .Select(p => new Attendee
                    {
                        UserId = p.UserId,
                        MeetingId = p.MeetingId,
                        IsModerator = p.IsModerator
                    });
                await _attendeeRepository.InsertManyAsync(attendeesInsert);
                return;
            }

            var newAttendeeIds = command.Attendees.Select(p => p.UserId).ToList();
            var tobeDeleteAttendeeIds = oldAttendeeIds.Except(newAttendeeIds);
            if (tobeDeleteAttendeeIds.Any())
            {
                await _attendeeRepository.DeleteAsync(x => tobeDeleteAttendeeIds.Contains(x.UserId) && x.MeetingId == command.MeetingId);
            }

            var newAttendees = command.Attendees
                .Where(x => !oldAttendeeIds.Contains(x.UserId))
                .Select(p => new Attendee
                {
                    UserId = p.UserId,
                    MeetingId = p.MeetingId,
                    IsModerator = p.IsModerator
                });
            if (newAttendees.Any())
            {
                await _attendeeRepository.InsertManyAsync(newAttendees);
            }
        }
    }
}
