using Thunder.Platform.Cqrs;

namespace Conexus.Opal.Microservice.Tagging.Events.ResourceSavedEvent
{
    public class SaveResourceEvent : BaseThunderEvent
    {
        public SaveResourceEvent(Domain.Entities.Resource resource)
        {
            Resource = resource;
        }

        public Domain.Entities.Resource Resource { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.metadata.resource.saved";
        }
    }
}
