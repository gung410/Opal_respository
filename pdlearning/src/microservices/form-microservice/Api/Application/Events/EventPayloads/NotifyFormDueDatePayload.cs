namespace Microservice.Form.Application.Events.EventPayloads
{
    public class NotifyFormDueDatePayload
    {
        public string UserName { get; set; }

        public string FormName { get; set; }

        public string FormDetailUrl { get; set; }

        public int RemindBeforeDays { get; set; }

        // this fields use for in app notification setting, please don't remove it
        public string ActionName { get; set; }

        public string ActionUrl { get; set; }
    }
}
