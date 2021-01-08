namespace Microservice.LnaForm.Application.Events.EventPayloads
{
    public class NotifyFormParticipantPayload
    {
        public string UserName { get; set; }

        public string CreatorName { get; set; }

        public string FormName { get; set; }

        public string FormDetailUrl { get; set; }
    }
}
