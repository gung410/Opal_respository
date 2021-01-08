using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microservice.Calendar.Domain.Extensions;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class UpdateCourseAssignmentBaseEventCommandHandler : BaseCommandHandler<UpdateCourseAssignmentBaseEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public UpdateCourseAssignmentBaseEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(UpdateCourseAssignmentBaseEventCommand command, CancellationToken cancellationToken)
        {
            var assignmentBaseEvent = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseAssignment && x.SourceId == command.AssignmentId && x.SourceParentId == command.ClassRunId);

            if (assignmentBaseEvent == null)
            {
                throw new EntityNotFoundException();
            }

            assignmentBaseEvent.Title = command.Title;
            assignmentBaseEvent.StartAt = command.StartAt;
            assignmentBaseEvent.EndAt = command.EndAt;
            assignmentBaseEvent.Status = command.Status;

            await _personalEventRepository.UpdateAsync(assignmentBaseEvent);
        }
    }
}
