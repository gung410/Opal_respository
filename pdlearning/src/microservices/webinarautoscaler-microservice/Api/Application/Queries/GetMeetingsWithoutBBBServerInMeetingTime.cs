using System.Collections.Generic;
using Microservice.WebinarAutoscaler.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Queries
{
    public class GetMeetingsWithoutBBBServerInMeetingTime : BaseThunderQuery<List<MeetingInfoModel>>
    {
    }
}
