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
    public class
        UpdateCourseAssignmentEventByIdsCommandHandler : BaseCommandHandler<
            UpdateCourseAssignmentEventByIdsCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public UpdateCourseAssignmentEventByIdsCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(
            UpdateCourseAssignmentEventByIdsCommand command,
            CancellationToken cancellationToken)
        {
            var assignmentEventExisted = await _personalEventRepository
                .GetAllListAsync(x => command.AssignmentEventIds.Contains(x.Id));

            if (assignmentEventExisted == null)
            {
                throw new EntityNotFoundException();
            }

            assignmentEventExisted.ForEach(x => x.Status = command.Status);
            await _personalEventRepository.UpdateManyAsync(assignmentEventExisted);
        }
    }
}
