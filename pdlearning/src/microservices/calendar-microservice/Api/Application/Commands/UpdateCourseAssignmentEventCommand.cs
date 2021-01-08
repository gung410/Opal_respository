using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Commands
{
    public class UpdateCourseAssignmentEventCommand : BaseCalendarCommand
    {
        /// <summary>
        /// Source Id.
        /// </summary>
        public Guid AssignmentParticipantTrackId { get; set; }

        public Guid UserId { get; set; }

        public Guid AssignmentId { get; set; }

        public CalendarEventSource Source { get; set; }

        public string Title { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public EventStatus Status { get; set; }
    }
}
