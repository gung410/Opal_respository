using Microservice.Content.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Events
{
    public class MigrateSearchContentDataEvent : BaseThunderEvent
    {
        public MigrateSearchContentDataEvent(DigitalContentModel model)
        {
            Model = model;
        }

        public DigitalContentModel Model { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.content.migrate";
        }
    }
}
