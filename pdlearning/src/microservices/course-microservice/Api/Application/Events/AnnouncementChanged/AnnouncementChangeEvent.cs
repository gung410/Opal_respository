using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class AnnouncementChangeEvent : BaseThunderEvent
    {
        public AnnouncementChangeEvent(Announcement announcement, AnnouncementChangeType changeType)
        {
            Announcement = announcement;
            ChangeType = changeType;
        }

        public Announcement Announcement { get; }

        public AnnouncementChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.announcement.{ChangeType.ToString().ToLower()}";
        }
    }
}
