using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Commands.CommandHandlers
{
    public class UpdateMeetingInfoBBBServerIdCommandHandler : BaseCommandHandler<UpdateMeetingInfoBBBServerIdCommand>
    {
        private readonly IRepository<MeetingInfo> _meetingRepository;
        private readonly IRepository<BBBServer> _bbbServerRepository;

        public UpdateMeetingInfoBBBServerIdCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<MeetingInfo> meetingRepository,
            IRepository<BBBServer> bbbServerRepository) : base(unitOfWorkManager)
        {
            _meetingRepository = meetingRepository;
            _bbbServerRepository = bbbServerRepository;
        }

        protected override async Task HandleAsync(UpdateMeetingInfoBBBServerIdCommand command, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.FirstOrDefaultAsync(x => x.Id == command.Id);

            if (meeting == null)
            {
                throw new EntityNotFoundException($"Meeting id: {command.Id} not found in db.");
            }

            meeting.BBBServerId = command.BBBServerId;
            await _meetingRepository.UpdateAsync(meeting);
        }
    }
}
