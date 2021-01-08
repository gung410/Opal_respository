namespace LearnerApp.Models.Newsfeed
{
    public class Forward : FeedBase
    {
        public int PostId { get; set; }

        public string Url { get; set; }

        public string CommunityId { get; set; }

        public string CommunityThumbnailUrl { get; set; }
    }
}
