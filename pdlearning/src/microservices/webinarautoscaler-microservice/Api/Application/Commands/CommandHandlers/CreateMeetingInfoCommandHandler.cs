using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarAutoscaler.Application.Commands.CommandHandlers
{
    public class CreateMeetingInfoCommandHandler : BaseCommandHandler<CreateMeetingInfoCommand>
    {
        private readonly IRepository<MeetingInfo> _meetingRepository;

        public CreateMeetingInfoCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<MeetingInfo> meetingRepository) : base(unitOfWorkManager)
        {
            _meetingRepository = meetingRepository;
        }

        protected override async Task HandleAsync(CreateMeetingInfoCommand command, CancellationToken cancellationToken)
        {
            var meetingInfo = new MeetingInfo()
            {
                Id = command.Id,
                Title = command.Title,
                PreRecordPath = command.PreRecordPath,
                PreRecordId = command.PreRecordId ?? null,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                IsCanceled = command.IsCanceled,
                ParticipantCount = command.ParticipantCount
            };

            await _meetingRepository.InsertAsync(meetingInfo);
        }
    }
}
