using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Commands
{
    public class ChangeAnnouncementStatusCommand : BaseThunderCommand
    {
        public AnnouncementStatus Status { get; set; }

        public SendAnnouncementCommandSearchCondition ForAnnouncements { get; set; } = new SendAnnouncementCommandSearchCondition();
    }

    public class SendAnnouncementCommandSearchCondition
    {
        public List<Guid> Ids { get; set; }

        public DateTime? ScheduleDateFrom { get; set; }

        public DateTime? ScheduleDateBefore { get; set; }

        public bool HasSpecificAnnouncementIds()
        {
            return Ids != null && Ids.Any();
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
