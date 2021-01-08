using System.Threading;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Events;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands.CommandHandlers
{
    public class SaveMeetingInfoCommandHandler : BaseCommandHandler<SaveMeetingInfoCommand>
    {
        private readonly IRepository<MeetingInfo> _meetingRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public SaveMeetingInfoCommandHandler(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IRepository<MeetingInfo> meetingRepository) : base(unitOfWorkManager, userContext)
        {
            _thunderCqrs = thunderCqrs;
            _meetingRepository = meetingRepository;
        }

        protected override async Task HandleAsync(SaveMeetingInfoCommand command, CancellationToken cancellationToken)
        {
            var meetingInfo = new MeetingInfo()
            {
                Id = command.Id,
                Title = command.Title,
                PreRecordPath = command.PreRecordPath,
                PreRecordId = command.PreRecordId ?? null,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                IsCanceled = command.IsCanceled
            };
            await _meetingRepository.InsertAsync(meetingInfo);

            await _thunderCqrs
             .SendEvent(
                     new MeetingChangeEvent(
                     new MeetingChangeModel(meetingInfo, command.ParticipantCount),
                     MeetingChangeType.Created));
        }
    }
}
