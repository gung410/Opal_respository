using System;

namespace Microservice.Calendar.Application.Commands
{
    public class CreateRegistrationCommand : BaseCalendarCommand
    {
        public Guid UserId { get; set; }

        public Guid ClassRunId { get; set; }

        public bool IsAccepted { get; set; }
    }
}
