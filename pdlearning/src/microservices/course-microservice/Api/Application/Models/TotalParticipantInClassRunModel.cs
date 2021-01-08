using System;

namespace Microservice.Course.Application.Models
{
    public class TotalParticipantInClassRunModel
    {
        public TotalParticipantInClassRunModel()
        {
        }

        public TotalParticipantInClassRunModel(Guid classRunId, int participantTotal = 0)
        {
            ClassRunId = classRunId;
            ParticipantTotal = participantTotal;
        }

        public Guid ClassRunId { get; set; }

        public int ParticipantTotal { get; set; }
    }
}
