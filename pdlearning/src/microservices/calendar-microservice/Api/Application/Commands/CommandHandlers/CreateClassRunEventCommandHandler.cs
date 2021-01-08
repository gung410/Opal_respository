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
    public class CreateClassRunEventCommandHandler : BaseCommandHandler<CreateClassRunEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public CreateClassRunEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(CreateClassRunEventCommand command, CancellationToken cancellationToken)
        {
            var classRunEventCreate = new PersonalEvent()
                .WithTitle(command.ClassTitle)
                .WithTime(command.StartDateTime, command.EndDateTime)
                .FromSource(CalendarEventSource.CourseClassRun, command.ClassRunId)
                .FromSourceParent(command.CourseId)
                .WithStatus(command.Status);

            await _personalEventRepository.InsertAsync(classRunEventCreate);
        }
    }
}
