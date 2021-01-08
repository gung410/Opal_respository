using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeClassRunRescheduleStatusRequest
    {
        public List<Guid> Ids { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public List<RescheduleSessionDto> RescheduleSessions { get; set; }

        public string Comment { get; set; }

        public ClassRunRescheduleStatus RescheduleStatus { get; set; }
    }
}
