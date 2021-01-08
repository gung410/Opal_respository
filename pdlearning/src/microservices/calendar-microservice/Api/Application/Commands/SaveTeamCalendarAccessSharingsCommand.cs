using System;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Commands
{
    public class SaveTeamCalendarAccessSharingsCommand : BaseThunderCommand
    {
        public Guid OwnerId { get; set; }

        public SaveTeamCalendarAccessSharingsRequest Request { get; set; }
    }
}
