using System;

namespace Microservice.Calendar.Application.Commands
{
    public class DeleteAttendeeExternalPdoEventCommand : BaseCalendarCommand
    {
        public Guid ExternalPdoId { get; set; }

        public Guid AttendeeId { get; set; }
    }
}
