using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public class MyDigitalContentChangeEvent : BaseThunderEvent
    {
        public MyDigitalContentChangeEvent(MyDigitalContent myDigitalContent, MyDigitalContentStatus changeType)
        {
            MyDigitalContent = myDigitalContent;
            ChangeType = changeType;
        }

        public MyDigitalContent MyDigitalContent { get; set; }

        public MyDigitalContentStatus ChangeType { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.learner.digitalcontent.{ChangeType.ToString().ToLower()}";
        }
    }
}
