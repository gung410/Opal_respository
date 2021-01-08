using Microservice.Form.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Events
{
    public enum FormParticipantChangeType
    {
        Created,
        Updated,
        Deleted
    }

    public class FormParticipantChangeEvent : BaseThunderEvent
    {
        public FormParticipantChangeEvent(FormParticipant formParticipant, FormParticipantChangeType type)
        {
            Type = type;
            FormParticipant = formParticipant;
        }

        public FormParticipant FormParticipant { get; }

        public FormParticipantChangeType Type { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.form.participant.{Type.ToString().ToLower()}";
        }
    }
}
