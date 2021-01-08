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
    public class UpdateLearningNeedsAnalysisEventCommandHandler : BaseCommandHandler<UpdateLearningNeedsAnalysisEventCommand>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public UpdateLearningNeedsAnalysisEventCommandHandler(
            IRepository<PersonalEvent> personalEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _personalEventRepository = personalEventRepository;
        }

        protected override async Task HandleAsync(UpdateLearningNeedsAnalysisEventCommand command, CancellationToken cancellationToken)
        {
            var eventExisted = await _personalEventRepository
               .FirstOrDefaultAsync(x => x.SourceId == command.LearningNeedsAnalysisId && x.Source == CalendarEventSource.LNA);

            if (eventExisted == null)
            {
                throw new EntityNotFoundException();
            }

            eventExisted.EndAt = command.EndAt;
            await _personalEventRepository.UpdateAsync(eventExisted);
        }
    }
}
