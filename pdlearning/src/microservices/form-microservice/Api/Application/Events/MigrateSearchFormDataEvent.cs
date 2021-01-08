using Microservice.Form.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Events
{
    public class MigrateSearchFormDataEvent : BaseThunderEvent
    {
        public MigrateSearchFormDataEvent(FormModel model)
        {
            Model = model;
        }

        public FormModel Model { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.form.migrate";
        }
    }
}
