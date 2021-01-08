using System;

namespace Microservice.Calendar.Application.Commands
{
    public class DeletePersonalEventCommand : BaseCalendarCommand
    {
        public Guid EventId { get; set; }

        public Guid? UserId { get; set; }
    }
}
