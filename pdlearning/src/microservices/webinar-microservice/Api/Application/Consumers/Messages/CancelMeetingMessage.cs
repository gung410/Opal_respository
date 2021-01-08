using System;
using Microservice.Webinar.Domain.Enums;

namespace Microservice.Webinar.Application.Consumers.Messages
{
    public class CancelMeetingMessage
    {
        public Guid SessionId { get; set; }

        public BookingSource Source { get; set; }
    }
}
