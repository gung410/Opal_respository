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
    public class CreateLearningNeedsAnalysisEventCommandHandler : BaseCommandHandler<CreateLearningNeedsAnalysisEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;

        public CreateLearningNeedsAnalysisEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
        }

        protected override async Task HandleAsync(CreateLearningNeedsAnalysisEventCommand command, CancellationToken cancellationToken)
        {
            var eventCreate = new PersonalEvent()
                .WithBasicInfo(command.Title, command.Description)
                .WithTime(
                    startAt: command.StartAt,
                    endAt: command.EndAt,
                    isAllDay: command.IsAllDay)
                .FromSource(CalendarEventSource.LNA, command.LearningNeedsAnalysisId);
            await _personalEventRepository.InsertAsync(eventCreate);

            var personalEvent = new UserPersonalEvent { EventId = eventCreate.Id, UserId = command.AttendeeId };
            await _userEventRepository.InsertAsync(personalEvent);
        }
    }
}
