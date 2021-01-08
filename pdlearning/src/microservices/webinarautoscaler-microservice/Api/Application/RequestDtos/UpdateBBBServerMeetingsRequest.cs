using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.WebinarAutoscaler.Application.RequestDtos
{
    public class UpdateBBBServerMeetingsRequest
    {
        public string InstanceId { get; set; }

        public List<string> MeetingIds { get; set; }
    }
}
