using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microservice.Calendar.Domain.Extensions;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class CreateExternalPdoEventCommandHandler : BaseCommandHandler<CreateExternalPdoEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;

        public CreateExternalPdoEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
        }

        protected override async Task HandleAsync(CreateExternalPdoEventCommand command, CancellationToken cancellationToken)
        {
            var externalPdoEventCreate = new PersonalEvent()
                .WithBasicInfo(command.Title, command.Description)
                .WithTime(
                    startAt: command.StartAt,
                    endAt: command.EndAt,
                    isAllDay: command.IsAllDay)
                .FromSource(CalendarEventSource.ExternalPDO, command.ExternalPdoId);
            await _personalEventRepository.InsertAsync(externalPdoEventCreate);

            var personalEvent = new UserPersonalEvent
            {
                EventId = externalPdoEventCreate.Id,
                UserId = command.AttendeeId,
                IsAccepted = command.IsAccepted
            };
            await _userEventRepository.InsertAsync(personalEvent);
        }
    }
}
