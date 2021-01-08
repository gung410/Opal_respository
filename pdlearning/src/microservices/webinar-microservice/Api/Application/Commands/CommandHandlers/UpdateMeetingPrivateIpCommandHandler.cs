using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands.CommandHandlers
{
    public class UpdateMeetingPrivateIpCommandHandler : BaseThunderCommandHandler<UpdateMeetingPrivateIpCommand>
    {
        private readonly IRepository<MeetingInfo> _meetingInfoRepo;

        public UpdateMeetingPrivateIpCommandHandler(IRepository<MeetingInfo> meetingInfoRepo)
        {
            _meetingInfoRepo = meetingInfoRepo;
        }

        protected override async Task HandleAsync(UpdateMeetingPrivateIpCommand command, CancellationToken cancellationToken)
        {
            var meetingInfo = await _meetingInfoRepo.GetAsync(command.MeetingId);
            if (meetingInfo == null)
            {
                throw new EntityNotFoundException(typeof(MeetingInfo), command.MeetingId);
            }

            meetingInfo.BBBServerPrivateIp = command.BBBServerPrivateIp;

            await _meetingInfoRepo.UpdateAsync(meetingInfo);
        }
    }
}
