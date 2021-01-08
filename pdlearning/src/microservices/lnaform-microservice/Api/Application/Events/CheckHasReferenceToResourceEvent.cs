using Microservice.LnaForm.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Events
{
    public class CheckHasReferenceToResourceEvent : BaseThunderEvent
    {
        public CheckHasReferenceToResourceEvent(CheckHasReferenceToResourceModel model)
        {
            Model = model;
        }

        public CheckHasReferenceToResourceModel Model { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.ccpm.has-reference-to-resource";
        }
    }
}
