using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Webinar.Application.Commands.CommandHandlers
{
    public class SaveAttendeesCommandHandler : BaseCommandHandler<SaveAttendeesCommand>
    {
        private readonly IRepository<Attendee> _attendeeRepository;

        public SaveAttendeesCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<Attendee> attendeeRepository) : base(unitOfWorkManager, userContext)
        {
            _attendeeRepository = attendeeRepository;
        }

        protected override Task HandleAsync(SaveAttendeesCommand command, CancellationToken cancellationToken)
        {
            if (command.Attendees == null || !command.Attendees.Any())
            {
                return Task.CompletedTask;
            }

            var attendeeList = command.Attendees
                .Select(p => new Attendee()
                {
                    MeetingId = p.MeetingId,
                    UserId = p.UserId,
                    IsModerator = p.IsModerator
                });

            return _attendeeRepository.InsertManyAsync(attendeeList);
        }
    }
}
