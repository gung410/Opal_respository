using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Events;
using Microservice.Webinar.Application.Exception;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands.CommandHandlers
{
    public class UpdateMeetingInfoCommandHandler : BaseCommandHandler<UpdateMeetingInfoCommand>
    {
        private readonly IRepository<MeetingInfo> _meetingRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public UpdateMeetingInfoCommandHandler(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<MeetingInfo> meetingRepository) : base(unitOfWorkManager, userContext)
        {
            _thunderCqrs = thunderCqrs;
            _meetingRepository = meetingRepository;
        }

        protected override async Task HandleAsync(UpdateMeetingInfoCommand command, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.FirstOrDefaultAsync(x => x.Id == command.Id);

            if (meeting == null)
            {
                throw new MeetingNotFoundException();
            }

            meeting.Title = command.Title;
            meeting.StartTime = command.StartTime;
            meeting.EndTime = command.EndTime;
            meeting.PreRecordPath = command.PreRecordPath;
            meeting.PreRecordId = command.PreRecordId;

            await _meetingRepository.UpdateAsync(meeting);

            await _thunderCqrs
                .SendEvent(
                        new MeetingChangeEvent(
                        new MeetingChangeModel(meeting, command.ParticipantCount),
                        MeetingChangeType.Updated));
        }
    }
}
