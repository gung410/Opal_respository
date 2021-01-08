using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.WebinarAutoscaler.Application.Commands;
using Microservice.WebinarAutoscaler.Application.Consumers.Messages;
using Microservice.WebinarAutoscaler.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Consumers
{
    [OpalConsumer("microservice.events.webinar.meeting-info.created")]
    [OpalConsumer("microservice.events.webinar.meeting-info.updated")]
    public class MeetingInfoChangedConsumer : ScopedOpalMessageConsumer<MeetingInfoChangedMessage>
    {
        public async Task InternalHandleAsync(
            MeetingInfoChangedMessage message,
            IThunderCqrs cqrs,
            IRepository<MeetingInfo> meetingRepository)
        {
            var meetingExist = await meetingRepository.FirstOrDefaultAsync(x => x.Id == message.Meeting.Id);

            if (meetingExist == null)
            {
                await cqrs.SendCommand(new CreateMeetingInfoCommand
                {
                    Id = message.Meeting.Id,
                    Title = message.Meeting.Title,
                    StartTime = message.Meeting.StartTime,
                    EndTime = message.Meeting.EndTime,
                    IsCanceled = message.Meeting.IsCanceled,
                    PreRecordPath = message.Meeting.PreRecordPath,
                    ParticipantCount = message.ParticipantCount,
                    PreRecordId = message.Meeting.PreRecordId ?? null
                });
                return;
            }

            await cqrs.SendCommand(new UpdateMeetingInfoCommand
            {
                Id = message.Meeting.Id,
                Title = message.Meeting.Title,
                StartTime = message.Meeting.StartTime,
                EndTime = message.Meeting.EndTime,
                IsCanceled = message.Meeting.IsCanceled,
                PreRecordPath = message.Meeting.PreRecordPath,
                ParticipantCount = message.ParticipantCount,
                PreRecordId = message.Meeting.PreRecordId ?? null,
                BBBServerId = null
            });
        }
    }
}
