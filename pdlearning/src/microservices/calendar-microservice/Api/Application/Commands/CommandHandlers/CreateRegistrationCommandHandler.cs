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
    public class CreateRegistrationCommandHandler : BaseCommandHandler<CreateRegistrationCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;

        public CreateRegistrationCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
        }

        protected override async Task HandleAsync(CreateRegistrationCommand command, CancellationToken cancellationToken)
        {
            var classRunEventExisted = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseClassRun && x.SourceId == command.ClassRunId);

            if (classRunEventExisted == null)
            {
                throw new EntityNotFoundException();
            }

            var userEvent = new UserPersonalEvent
            {
                UserId = command.UserId,
                EventId = classRunEventExisted.Id,
                IsAccepted = command.IsAccepted
            };

            await _userEventRepository.InsertAsync(userEvent);
        }
    }
}
