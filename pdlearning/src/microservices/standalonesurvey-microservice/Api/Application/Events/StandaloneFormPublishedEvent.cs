using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Events
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
            return $"microservice.events.standalonesurvey.standalone-form.published";
        }
    }
}
