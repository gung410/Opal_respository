namespace Microservice.Form.Application.Events.EventPayloads
{
    public class FormNotifySubmittedRequestPayload
    {
        public string RecipientName { get; set; }

        public string CreatorName { get; set; }

        public string FormName { get; set; }

        public string FormType { get; set; }

        public string AOName { get; set; }

        public string Comment { get; set; }

        public string FormDetailUrl { get; set; }
    }
}
