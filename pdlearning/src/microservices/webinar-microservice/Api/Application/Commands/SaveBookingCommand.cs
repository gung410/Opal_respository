using System;
using Microservice.Webinar.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands
{
    public class SaveBookingCommand : BaseThunderCommand
    {
        public Guid MeetingId { get; set; }

        public BookingSource Source { get; set; }

        public Guid SourceId { get; set; }
    }
}
