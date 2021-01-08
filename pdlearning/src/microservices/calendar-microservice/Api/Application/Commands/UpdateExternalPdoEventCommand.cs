using System;

namespace Microservice.Calendar.Application.Commands
{
    public class UpdateExternalPdoEventCommand : BaseCalendarCommand
    {
        public Guid ExternalPdoId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public bool IsAccepted { get; set; }

        public Guid AttendeeId { get; set; }
    }
}
