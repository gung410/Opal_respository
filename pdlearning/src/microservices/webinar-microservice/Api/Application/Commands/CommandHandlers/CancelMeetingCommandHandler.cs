using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Events;
using Microservice.Webinar.Application.Exception;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands.CommandHandlers
{
    public class CancelMeetingCommandHandler : BaseCommandHandler<CancelMeetingCommand>
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<MeetingInfo> _meetingRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public CancelMeetingCommandHandler(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<Booking> bookingRepository,
            IRepository<MeetingInfo> meetingRepository) : base(unitOfWorkManager, userContext)
        {
            _thunderCqrs = thunderCqrs;
            _bookingRepository = bookingRepository;
            _meetingRepository = meetingRepository;
        }

        protected override async Task HandleAsync(CancelMeetingCommand command, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.FirstOrDefaultAsync(x => x.SourceId == command.SourceId && x.Source == command.Source);
            if (booking == null)
            {
                throw new BookingNotFoundException();
            }

            var meeting = await _meetingRepository.FirstOrDefaultAsync(x => x.Id == booking.MeetingId);
            if (meeting == null)
            {
                throw new MeetingNotFoundException();
            }

            meeting.IsCanceled = true;

            await _meetingRepository.UpdateAsync(meeting);
            await _thunderCqrs
                .SendEvent(
                new MeetingChangeEvent(
                new MeetingChangeModel(meeting, 0),
                MeetingChangeType.Updated));
        }
    }
}
