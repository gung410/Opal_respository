using Microservice.StandaloneSurvey.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Events
{
    public class FormSubmitEvent : BaseThunderEvent
    {
        public FormSubmitEvent(FormSubmitEventModel formSubmitModel)
        {
            FormSubmitModel = formSubmitModel;
        }

        public FormSubmitEventModel FormSubmitModel { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.standalonesurvey.submit";
        }
    }
}
