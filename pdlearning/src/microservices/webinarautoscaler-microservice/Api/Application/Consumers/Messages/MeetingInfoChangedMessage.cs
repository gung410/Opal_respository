using System;
using Microservice.WebinarAutoscaler.Domain.Entities;

namespace Microservice.WebinarAutoscaler.Application.Consumers.Messages
{
    public class MeetingInfoChangedMessage
    {
        public MeetingInfo Meeting { get; set; }

        public int ParticipantCount { get; set; }
    }
}
