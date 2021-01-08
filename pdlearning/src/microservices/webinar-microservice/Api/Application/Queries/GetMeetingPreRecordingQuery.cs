using System;
using System.Collections.Generic;
using Microservice.Webinar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries
{
    public class GetMeetingPreRecordingQuery : BaseThunderQuery<List<ResultGetMeetingPreRecordingModel>>
    {
        public List<Guid> MeetingIds { get; set; }
    }
}
