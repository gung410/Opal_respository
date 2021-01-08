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
    public class CreateCourseAssignmentBaseEventCommandHandler : BaseCommandHandler<CreateCourseAssignmentBaseEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;

        public CreateCourseAssignmentBaseEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<UserPersonalEvent> userEventRepository) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
        }

        protected override async Task HandleAsync(CreateCourseAssignmentBaseEventCommand command, CancellationToken cancellationToken)
        {
            var assignmentEvent = new PersonalEvent()
                .WithTitle(command.Title)
                .WithTime(command.StartAt, command.EndAt)
                .FromSource(CalendarEventSource.CourseAssignment, command.AssignmentId)
                .FromSourceParent(command.ClassRunId)
                .WithStatus(command.Status);
            assignmentEvent.RepeatFrequency = RepeatFrequency.None;

            await _personalEventRepository.InsertAsync(assignmentEvent);
        }
    }
}
