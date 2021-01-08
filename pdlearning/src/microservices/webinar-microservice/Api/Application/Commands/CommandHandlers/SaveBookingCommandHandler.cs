using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Webinar.Application.Commands.CommandHandlers
{
    public class SaveBookingCommandHandler : BaseCommandHandler<SaveBookingCommand>
    {
        private readonly IRepository<Booking> _bookingRepository;

        public SaveBookingCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<Booking> bookingRepository) : base(unitOfWorkManager, userContext)
        {
            _bookingRepository = bookingRepository;
        }

        protected override Task HandleAsync(SaveBookingCommand command, CancellationToken cancellationToken)
        {
            var booking = new Booking()
            {
                MeetingId = command.MeetingId,
                Source = command.Source,
                SourceId = command.SourceId
            };

            return _bookingRepository.InsertAsync(booking);
        }
    }
}
