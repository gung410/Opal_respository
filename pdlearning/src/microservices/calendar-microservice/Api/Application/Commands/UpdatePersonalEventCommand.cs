using System;
using Microservice.Calendar.Application.RequestDtos;

namespace Microservice.Calendar.Application.Commands
{
    public class UpdatePersonalEventCommand : BaseCalendarCommand
    {
        public UpdatePersonalEventRequest Request { get; set; }

        public Guid? UserId { get; set; }
    }
}
