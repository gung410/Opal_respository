using System;

namespace Microservice.Webinar.Application.Models
{
    public class MeetingInfoModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string BBBServerPrivateIp { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsCanceled { get; set; }
    }
}
