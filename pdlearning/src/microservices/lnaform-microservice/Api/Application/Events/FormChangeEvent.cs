using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.LnaForm.Application.Events
{
    public enum FormChangeType
    {
        Created,
        Deleted,
        Updated,
        Archived,
        Rollback
    }

    public class FormChangeEvent : BaseThunderEvent
    {
        public FormChangeEvent(Domain.Entities.Form form, FormChangeType changeType)
        {
            Form = form;
            ChangeType = changeType;
        }

        public Domain.Entities.Form Form { get; }

        public FormChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.lnaform.{ChangeType.ToString().ToLower()}";
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
