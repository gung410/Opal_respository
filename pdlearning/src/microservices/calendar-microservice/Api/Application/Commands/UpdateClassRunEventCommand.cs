using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Commands
{
    public class UpdateClassRunEventCommand : BaseCalendarCommand
    {
        public Guid ClassRunId { get; set; }

        public string ClassTitle { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public EventStatus Status { get; set; }

        public Guid CourseId { get; set; }
    }
}
