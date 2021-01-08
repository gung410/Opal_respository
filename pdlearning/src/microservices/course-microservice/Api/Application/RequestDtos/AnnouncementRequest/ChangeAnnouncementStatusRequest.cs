using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeAnnouncementStatusRequest
    {
        public AnnouncementStatus Status { get; set; }

        public List<Guid> Ids { get; set; }
    }
}
