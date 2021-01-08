using System;
using Microservice.Webinar.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries
{
    public class CheckBookingExistsQuery : BaseThunderQuery<bool>
    {
        public Guid SessionId { get; set; }

        public BookingSource Source { get; set; }
    }
}
