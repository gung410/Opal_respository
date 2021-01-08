using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteAnnouncementTemplateCommand : BaseThunderCommand
    {
        public Guid AnnouncementTemplateId { get; set; }
    }
}
