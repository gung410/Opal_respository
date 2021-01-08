using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Events
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
            return $"microservice.events.form.standalone-form.published";
        }
    }
}
