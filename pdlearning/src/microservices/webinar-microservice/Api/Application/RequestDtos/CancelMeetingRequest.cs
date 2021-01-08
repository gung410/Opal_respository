using System;
using Microservice.Webinar.Domain.Enums;

namespace Microservice.Webinar.Application.RequestDtos
{
    public class CancelMeetingRequest
    {
        public Guid SessionId { get; set; }

        public BookingSource Source { get; set; }
    }
}
