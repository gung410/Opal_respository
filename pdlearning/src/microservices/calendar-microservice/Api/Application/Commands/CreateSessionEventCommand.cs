using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Commands
{
    public class CreateSessionEventCommand : BaseCalendarCommand
    {
        public Guid SessionId { get; set; }

        public Guid ClassRunId { get; set; }

        public string Title { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public EventStatus Status { get; set; }
    }
}
