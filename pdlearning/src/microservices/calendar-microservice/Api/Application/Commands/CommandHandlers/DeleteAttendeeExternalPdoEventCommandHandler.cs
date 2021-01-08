using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class DeleteAttendeeExternalPdoEventCommandHandler : BaseCommandHandler<DeleteAttendeeExternalPdoEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;

        public DeleteAttendeeExternalPdoEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
        }

        protected override async Task HandleAsync(DeleteAttendeeExternalPdoEventCommand command, CancellationToken cancellationToken)
        {
            var eventExisted = await _personalEventRepository
               .FirstOrDefaultAsync(e => e.SourceId == command.ExternalPdoId && e.Source == CalendarEventSource.ExternalPDO);
            if (eventExisted == null)
            {
                throw new EntityNotFoundException($"ExternalPdo event with SourceId {eventExisted.SourceId} was not existed.");
            }

            var attendeeExisted = await _userEventRepository
                .FirstOrDefaultAsync(x => x.UserId == command.AttendeeId && x.EventId == eventExisted.Id);
            await _userEventRepository.DeleteAsync(attendeeExisted);

            var listAttendeeExisted = await _userEventRepository
                .GetAll()
                .Where(ue => ue.EventId == eventExisted.Id)
                .Select(ue => ue)
                .ToListAsync();
            if (!listAttendeeExisted.Any())
            {
                await _personalEventRepository.DeleteAsync(eventExisted);
            }
        }
    }
}
