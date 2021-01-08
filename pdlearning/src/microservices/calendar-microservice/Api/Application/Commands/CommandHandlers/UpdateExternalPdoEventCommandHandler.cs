using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class UpdateExternalPdoEventCommandHandler : BaseCommandHandler<UpdateExternalPdoEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;

        public UpdateExternalPdoEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
        }

        protected override async Task HandleAsync(UpdateExternalPdoEventCommand command, CancellationToken cancellationToken)
        {
            var eventExisted = await _personalEventRepository
               .FirstOrDefaultAsync(x => x.SourceId == command.ExternalPdoId && x.Source == CalendarEventSource.ExternalPDO);
            if (eventExisted == null)
            {
                throw new EntityNotFoundException();
            }

            eventExisted.Title = command.Title;
            eventExisted.Description = command.Description;
            eventExisted.StartAt = command.StartAt;
            eventExisted.EndAt = command.EndAt;
            await _personalEventRepository.UpdateAsync(eventExisted);

            var attendeeExisted = await _userEventRepository
                .FirstOrDefaultAsync(x => x.UserId == command.AttendeeId && x.EventId == eventExisted.Id);
            if (attendeeExisted == null)
            {
                var personalEvent = new UserPersonalEvent
                {
                    EventId = eventExisted.Id,
                    UserId = command.AttendeeId,
                    IsAccepted = command.IsAccepted
                };
                await _userEventRepository.InsertAsync(personalEvent);
                return;
            }

            attendeeExisted.IsAccepted = command.IsAccepted;
            await _userEventRepository.UpdateAsync(attendeeExisted);
        }
    }
}
