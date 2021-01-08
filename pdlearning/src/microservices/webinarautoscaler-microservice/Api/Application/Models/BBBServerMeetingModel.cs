using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.WebinarAutoscaler.Domain.Entities;

namespace Microservice.WebinarAutoscaler.Application.Models
{
    public class BBBServerMeetingModel
    {
        public BBBServer BBBServer { get; set; }

        public MeetingInfo Meeting { get; set; }
    }
}
