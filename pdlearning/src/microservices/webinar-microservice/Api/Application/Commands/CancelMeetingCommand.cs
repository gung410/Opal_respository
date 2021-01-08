using System;
using Microservice.Webinar.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands
{
    public class CancelMeetingCommand : BaseThunderCommand
    {
        public Guid SourceId { get; set; }

        public BookingSource Source { get; set; }
    }
}
