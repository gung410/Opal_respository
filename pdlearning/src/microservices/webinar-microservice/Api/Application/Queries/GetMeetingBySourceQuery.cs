using System;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries
{
    public class GetMeetingBySourceQuery : BaseThunderQuery<MeetingInfoModel>
    {
        public BookingSource Source { get; set; }

        public Guid SourceId { get; set; }
    }
}
