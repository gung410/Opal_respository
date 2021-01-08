using System;

namespace Microservice.Calendar.Application.Commands
{
    public class DeleteRegistrationCommand : BaseCalendarCommand
    {
        public Guid UserId { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
