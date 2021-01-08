using Microservice.NewsFeed.Domain.Entities;

namespace Microservice.NewsFeed.Application.Models
{
    public class CommunityFeedModel : UserPostFeedModel
    {
        public CommunityFeedModel(CommunityPostFeed communityFeed) : base(communityFeed)
        {
            CommunityName = communityFeed.CommunityName;
            CommunityThumbnailUrl = communityFeed.CommunityThumbnailUrl;
            Description = communityFeed.Description;
        }

        public string Description { get; set; }

        public string CommunityName { get; set; }

        public string CommunityThumbnailUrl { get; set; }
    }
}
