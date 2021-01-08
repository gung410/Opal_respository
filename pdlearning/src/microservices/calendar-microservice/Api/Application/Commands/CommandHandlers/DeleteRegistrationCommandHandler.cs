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
    public class DeleteRegistrationCommandHandler : BaseCommandHandler<DeleteRegistrationCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;

        public DeleteRegistrationCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _userEventRepository = userEventRepository;
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(DeleteRegistrationCommand command, CancellationToken cancellationToken)
        {
            var classRunEventExisted = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseClassRun && x.SourceId == command.ClassRunId);

            if (classRunEventExisted == null)
            {
                throw new EntityNotFoundException();
            }

            var userEventExisted = await _userEventRepository
                .FirstOrDefaultAsync(x => x.UserId == command.UserId && x.EventId == classRunEventExisted.Id);

            await _userEventRepository.DeleteAsync(userEventExisted);
        }
    }
}
