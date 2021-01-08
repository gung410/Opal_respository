using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Form.Application.Events
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
        public FormChangeEvent(Form.Domain.Entities.Form form, FormChangeType changeType)
        {
            Form = form;
            ChangeType = changeType;
        }

        public Form.Domain.Entities.Form Form { get; }

        public FormChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.form.{ChangeType.ToString().ToLower()}";
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
