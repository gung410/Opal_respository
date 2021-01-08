using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Extensions
{
    public static class EventTypeExtension
    {
        public static string ToEventTypeName(this EventType eventType, ArchetypeEnum archetype)
        {
            return string.Format("{0}_{1}", archetype.ToString().ToUpper(), eventType.ToString().ToUpper());
        }
        public static string ToEventTypeName(this EventType eventType, string objectName)
        {
            return string.Format("{0}_{1}", objectName.ToUpper(), eventType.ToString().ToUpper());
        }
        public static string ToEventName(this EventType eventType, object objectName)
        {
            return string.Format("{0}.{1}", eventType.ToString().ToLower(), objectName.ToString().ToLower());
        }
    }
}
