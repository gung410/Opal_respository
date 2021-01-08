using System;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Commands
{
    public class CreatePersonalEventCommand : BaseCalendarCommand
    {
        public CreatePersonalEventRequest CreationRequest { get; set; }

        /// <summary>
        /// The event creator.
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Specify the user that the event will be added to his/her calendar.
        /// </summary>
        public Guid? UserId { get; set; }

        public CalendarEventSource Source { get; set; }

        public Guid? SourceId { get; set; }
    }
}
