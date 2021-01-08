namespace LearnerApp.Models.MyLearning
{
    public class TrackingInfo
    {
        public string ItemId { get; set; }

        public string ItemType { get; set; }

        public bool IsLike { get; set; }

        public int TotalLike { get; set; }

        public int TotalShare { get; set; }

        public int TotalView { get; set; }

        public int TotalDownload { get; set; }
    }
}
