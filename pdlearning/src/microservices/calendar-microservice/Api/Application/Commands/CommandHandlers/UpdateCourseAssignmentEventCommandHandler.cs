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
    public class UpdateCourseAssignmentEventCommandHandler : BaseCommandHandler<UpdateCourseAssignmentEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public UpdateCourseAssignmentEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(UpdateCourseAssignmentEventCommand command, CancellationToken cancellationToken)
        {
            var assignmentEventExisted = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseAssignment && x.SourceId == command.AssignmentParticipantTrackId && x.SourceParentId == command.AssignmentId);

            if (assignmentEventExisted == null)
            {
                throw new EntityNotFoundException();
            }

            assignmentEventExisted.Title = command.Title;
            assignmentEventExisted.StartAt = command.StartAt;
            assignmentEventExisted.EndAt = command.EndAt;
            assignmentEventExisted.Status = command.Status;

            await _personalEventRepository.UpdateAsync(assignmentEventExisted);
        }
    }
}
