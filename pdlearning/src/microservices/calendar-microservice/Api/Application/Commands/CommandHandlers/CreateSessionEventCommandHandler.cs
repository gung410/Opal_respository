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
    public class CreateSessionEventCommandHandler : BaseCommandHandler<CreateSessionEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public CreateSessionEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(CreateSessionEventCommand command, CancellationToken cancellationToken)
        {
            var sessionEvent = new PersonalEvent()
                .WithTitle(command.Title)
                .WithTime(command.StartDateTime, command.EndDateTime)
                .FromSource(CalendarEventSource.CourseSession, command.SessionId)
                .FromSourceParent(command.ClassRunId)
                .WithStatus(command.Status);

            await _personalEventRepository.InsertAsync(sessionEvent);
        }
    }
}
