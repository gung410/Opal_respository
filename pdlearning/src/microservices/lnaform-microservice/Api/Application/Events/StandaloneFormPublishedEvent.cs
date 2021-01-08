using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Events
{
    public class StandaloneFormPublishedEvent : BaseThunderEvent
    {
        public StandaloneFormPublishedEvent(StandaloneFormPublishedEventModel model)
        {
            Model = model;
        }

        public StandaloneFormPublishedEventModel Model { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.lnaform.standalone-form.published";
        }
    }
}
