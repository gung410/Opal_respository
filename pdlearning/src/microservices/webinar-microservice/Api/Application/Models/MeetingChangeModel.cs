using System;
using Microservice.Webinar.Domain.Entities;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Webinar.Application.Models
{
    public class MeetingChangeModel
    {
        public MeetingChangeModel(MeetingInfo meeting, int participantCount)
        {
            Meeting = meeting;
            ParticipantCount = participantCount;
        }

        public MeetingInfo Meeting { get; set; }

        public int ParticipantCount { get; set; }
    }
}
