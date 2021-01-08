using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Queries
{
    public class GetMeetingsByBBBServerIdQuery : BaseThunderQuery<List<MeetingInfoModel>>
    {
        public Guid? BBBServerId { get; set; }
    }
}
