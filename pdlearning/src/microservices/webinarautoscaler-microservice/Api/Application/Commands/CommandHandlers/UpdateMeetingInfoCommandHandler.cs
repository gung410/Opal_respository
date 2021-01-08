using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Commands.CommandHandlers
{
    public class UpdateMeetingInfoCommandHandler : BaseCommandHandler<UpdateMeetingInfoCommand>
    {
        private readonly IRepository<MeetingInfo> _meetingRepository;

        public UpdateMeetingInfoCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<MeetingInfo> meetingRepository) : base(unitOfWorkManager)
        {
            _meetingRepository = meetingRepository;
        }

        protected override async Task HandleAsync(UpdateMeetingInfoCommand command, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.FirstOrDefaultAsync(x => x.Id == command.Id);

            if (meeting == null)
            {
                throw new EntityNotFoundException($"Meeting id: {command.Id} not found in db.");
            }

            meeting.Title = command.Title;
            meeting.StartTime = command.StartTime;
            meeting.EndTime = command.EndTime;
            meeting.PreRecordPath = command.PreRecordPath;
            meeting.PreRecordId = command.PreRecordId ?? null;
            meeting.IsCanceled = command.IsCanceled;
            meeting.BBBServerId = command.BBBServerId;
            meeting.ParticipantCount = command.ParticipantCount;

            await _meetingRepository.UpdateAsync(meeting);
        }
    }
}
