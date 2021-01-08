using System;

namespace Microservice.NewsFeed.Domain.Entities
{
    public class CommunityPostFeed : UserPostFeed
    {
        /// <summary>
        /// The identifier community.
        /// </summary>
        public Guid CommunityId { get; set; }

        /// <summary>
        /// Description of the community.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Name of the community.
        /// </summary>
        public string CommunityName { get; set; }

        /// <summary>
        /// Thumbnail of the community.
        /// </summary>
        public string CommunityThumbnailUrl { get; set; }

        public override string Type => nameof(CommunityPostFeed);
    }
}
