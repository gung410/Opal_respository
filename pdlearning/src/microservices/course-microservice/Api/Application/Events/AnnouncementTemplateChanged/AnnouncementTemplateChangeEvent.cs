using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class AnnouncementTemplateChangeEvent : BaseThunderEvent
    {
        public AnnouncementTemplateChangeEvent(AnnouncementTemplate announcementTemplate, AnnouncementTemplateChangeType changeType)
        {
            AnnouncementTemplate = announcementTemplate;
            ChangeType = changeType;
        }

        public AnnouncementTemplate AnnouncementTemplate { get; }

        public AnnouncementTemplateChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.announcementtemplate.{ChangeType.ToString().ToLower()}";
        }
    }
}
