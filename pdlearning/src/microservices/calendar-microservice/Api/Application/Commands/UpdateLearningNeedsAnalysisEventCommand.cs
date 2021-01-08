using System;

namespace Microservice.Calendar.Application.Commands
{
    public class UpdateLearningNeedsAnalysisEventCommand : BaseCalendarCommand
    {
        public Guid LearningNeedsAnalysisId { get; set; }

        public DateTime EndAt { get; set; }
    }
}
