using System;

namespace Microservice.Calendar.Application.Commands
{
    public class CreateLearningNeedsAnalysisEventCommand : BaseCalendarCommand
    {
        public Guid LearningNeedsAnalysisId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public bool IsAllDay { get; set; }

        public Guid AttendeeId { get; set; }
    }
}
