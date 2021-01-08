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
    public class DeleteSessionEventCommandHandler : BaseCommandHandler<DeleteSessionEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public DeleteSessionEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(DeleteSessionEventCommand command, CancellationToken cancellationToken)
        {
            var sessionEventExisted = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.SourceId == command.SessionId && x.Source == CalendarEventSource.CourseSession && x.SourceParentId == command.ClassRunId);

            if (sessionEventExisted == null)
            {
                throw new EntityNotFoundException();
            }

            await _personalEventRepository.DeleteAsync(sessionEventExisted);
        }
    }
}
