using System;

namespace Microservice.Calendar.Application.Commands
{
    public class DeleteSessionEventCommand : BaseCalendarCommand
    {
        public Guid SessionId { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
