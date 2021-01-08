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
    public class UpdateClassRunEventCommandHandler : BaseCommandHandler<UpdateClassRunEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public UpdateClassRunEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(UpdateClassRunEventCommand command, CancellationToken cancellationToken)
        {
            var eventExisted = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.SourceId == command.ClassRunId && x.Source == CalendarEventSource.CourseClassRun);
            if (eventExisted == null)
            {
                throw new EntityNotFoundException();
            }

            eventExisted.Title = command.ClassTitle;
            eventExisted.StartAt = command.StartDateTime;
            eventExisted.EndAt = command.EndDateTime;
            eventExisted.Status = command.Status;
            eventExisted.SourceParentId = command.CourseId;

            await _personalEventRepository.UpdateAsync(eventExisted);
        }
    }
}
