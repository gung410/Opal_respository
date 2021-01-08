using System;

namespace Microservice.WebinarAutoscaler.Application.Models
{
    public class ScaleInModel
    {
        public Guid BBBServerId { get; internal set; }

        public int MeetingCount { get; internal set; }
    }
}
