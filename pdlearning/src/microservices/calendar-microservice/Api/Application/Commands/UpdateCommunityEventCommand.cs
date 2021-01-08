using System;
using Microservice.Calendar.Application.RequestDtos;

namespace Microservice.Calendar.Application.Commands
{
    public class UpdateCommunityEventCommand : BaseCalendarCommand
    {
        public UpdateCommunityEventRequest Request { get; set; }

        public Guid? UserId { get; set; }
    }
}
