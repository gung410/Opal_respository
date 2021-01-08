using System;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Webinar.Application.Events;
using Microservice.Webinar.Application.RequestDtos;
using Microservice.Webinar.Domain.Entities;
using Microservice.Webinar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Consumers.Records
{
    [OpalConsumer("microservice.events.webinar-record-converter.converting-tracking.new")]
    [OpalConsumer("microservice.events.webinar-record-converter.converting-tracking.ignoreretry")]
    [OpalConsumer("microservice.events.webinar-record-converter.converting-tracking.ready")]
    public class WebinarRecordChangedConsumer : ScopedOpalMessageConsumer<WebinarRecordChangedMessage>
    {
        public async Task InternalHandleAsync(
            WebinarRecordChangedMessage message,
            IRepository<Record> recordRepository,
            IRepository<MeetingInfo> meetingRepository,
            IRepository<Attendee> attendeeRepository,
            IThunderCqrs thunderCqrs)
        {
            var record = await recordRepository.FirstOrDefaultAsync(r => r.MeetingId == message.MeetingId
                                                                         && r.InternalMeetingId == message.InternalMeetingId);
            if (record == null)
            {
                record = new Record
                {
                    Id = message.RecordId,                  // The same as record id for easy to debug.
                    DigitalContentId = message.RecordId,    // The same as record id for easy to debug.
                    RecordId = message.RecordId,
                    MeetingId = message.MeetingId,
                    InternalMeetingId = message.InternalMeetingId,
                    Status = message.Status
                };
                await recordRepository.InsertAsync(record);
            }
            else
            {
                record.Status = message.Status;
                await recordRepository.UpdateAsync(record);
            }

            // Save the record metadata to CCPM.
            if (record.Status == RecordStatus.Ready)
            {
                var meeting = await meetingRepository.FirstOrDefaultAsync(message.MeetingId);
                var moderators = await attendeeRepository
                    .GetAll()
                    .Where(x => x.MeetingId == message.MeetingId && x.IsModerator)
                    .ToListAsync();
                var owner = moderators.FirstOrDefault();
                var ownerId = owner?.UserId ?? Guid.Empty;

                var collaboratorIds = moderators
                    .Where(x => x.Id != ownerId)
                    .Select(x => x.UserId)
                    .ToList();

                var payload = new SaveUploadedContentRequest
                {
                    Id = record.DigitalContentId,
                    OwnerId = ownerId,
                    CollaboratorIds = collaboratorIds,
                    FileSize = message.FileSize,
                    FileName = message.VideoPath,
                    FileExtension = message.Extension,
                    FileLocation = message.VideoPath,
                    Title = meeting.Title,
                    FileType = "video/mp4",
                    Type = "UploadedContent",
                    Status = "ReadyToUse",
                };

                await thunderCqrs.SendEvent(new RecordChangeEvent(payload));
            }
        }
    }
}
