using System;

namespace Microservice.Webinar.Application.Models
{
    public class BigBlueButtonMeetingInfoModel
    {
        public string MeetingId { get; set; }

        public string MeetingName { get; set; }

        public int ParticipantCount { get; set; }

        public bool Running { get; set; }

        public bool HasUserJoined { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
