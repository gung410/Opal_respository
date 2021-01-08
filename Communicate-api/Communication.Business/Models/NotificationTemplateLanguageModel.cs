namespace Communication.Business.Models
{
    public class NotificationTemplateLanguageModel
    {
        public string Id { get; set; }
        public string NotificationTemplateId { get; set; }
        public string TemplateContent { get; set; }
        public string TemplateSubject { get; set; }
        public string LanguageCode { get; set; }
    }
}