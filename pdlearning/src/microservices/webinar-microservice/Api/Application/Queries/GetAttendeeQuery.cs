using System;
using Microservice.Webinar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries
{
    public class GetAttendeeQuery : BaseThunderQuery<AttendeeModel>
    {
        public Guid MeetingId { get; set; }

        public Guid UserId { get; set; }
    }
}
