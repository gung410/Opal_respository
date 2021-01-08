using System;

namespace Microservice.Webinar.Application.Models
{
    public class ResultGetMeetingPreRecordingModel
    {
        public Guid MeetingId { get; set; }

        public string PreRecordUrl { get; set; }
    }
}
