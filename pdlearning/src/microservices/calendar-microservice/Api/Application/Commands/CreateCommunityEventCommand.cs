using System;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Commands
{
    public class CreateCommunityEventCommand : BaseCalendarCommand
    {
        public CreateCommunityEventRequest CreationRequest { get; set; }

        /// <summary>
        /// The event creator.
        /// </summary>
        public Guid CreatedBy { get; set; }

        public CalendarEventSource Source { get; set; }
    }
}
