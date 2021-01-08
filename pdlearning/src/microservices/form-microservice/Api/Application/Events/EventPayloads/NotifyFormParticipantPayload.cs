namespace Microservice.Form.Application.Events.EventPayloads
{
    public class NotifyFormParticipantPayload : BaseTodoEventPayload
    {
        public string UserName { get; set; }

        public string CreatorName { get; set; }

        public string FormName { get; set; }

        public string FormDetailUrl { get; set; }
    }
}
