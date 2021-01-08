using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.WebinarAutoscaler.Application.RequestDtos
{
    public class MeetingInfoRequest
    {
        public Guid MeetingId { get; set; }

        public string BBBServerPrivateIp { get; set; }
    }
}
