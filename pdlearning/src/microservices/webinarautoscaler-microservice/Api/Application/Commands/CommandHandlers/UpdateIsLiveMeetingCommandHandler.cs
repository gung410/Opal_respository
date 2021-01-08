using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Commands.CommandHandlers
{
    public class UpdateIsLiveMeetingCommandHandler : BaseCommandHandler<UpdateIsLiveMeetingCommand>
    {
        private readonly IRepository<MeetingInfo> _meetingRepository;

        public UpdateIsLiveMeetingCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<MeetingInfo> meetingRepository) : base(unitOfWorkManager)
        {
            _meetingRepository = meetingRepository;
        }

        protected override async Task HandleAsync(UpdateIsLiveMeetingCommand command, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.FirstOrDefaultAsync(x => x.Id == command.Id);

            if (meeting == null)
            {
                throw new EntityNotFoundException($"Meeting id: {command.Id} not found in db.");
            }

            meeting.IsLive = command.IsLive;

            await _meetingRepository.UpdateAsync(meeting);
        }
    }
}
