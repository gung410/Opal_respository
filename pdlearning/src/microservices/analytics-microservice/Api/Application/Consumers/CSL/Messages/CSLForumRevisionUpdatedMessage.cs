namespace Microservice.Analytics.Application.Consumers.CSL.Messages
{
    public class CSLForumRevisionUpdatedMessage
    {
        public int? Id { get; set; }

        public byte IsLatest { get; set; }

        public string Content { get; set; }
    }
}
