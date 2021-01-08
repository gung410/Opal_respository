using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Calendar.Application.Commands.CommandHandlers
{
    public class DeleteCourseAssignmentEventByIdsCommandHandler : BaseCommandHandler<DeleteCourseAssignmentEventByIdsCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public DeleteCourseAssignmentEventByIdsCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(DeleteCourseAssignmentEventByIdsCommand command, CancellationToken cancellationToken)
        {
            var courseAssignmentEvents = await _personalEventRepository
                .GetAllListAsync(x => command.EventIds.Contains(x.Id));

            if (courseAssignmentEvents.Count == 0)
            {
                throw new EntityNotFoundException();
            }

            await _personalEventRepository.DeleteManyAsync(courseAssignmentEvents);
        }
    }
}
