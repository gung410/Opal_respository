namespace LearnerApp.Models.Sharing
{
    public class ShareContentArgumentsPayload
    {
        public string ItemId { get; set; }

        public string ItemType { get; set; }

        public string[] SharedUsers { get; set; }
    }
}
