using System;
using Microservice.NewsFeed.Domain.ValueObject;

namespace Microservice.NewsFeed.Domain.Entities
{
    public class UserPostForward
    {
        /// <summary>
        /// The identifier post.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// The content of the post.
        /// </summary>
        public string PostContent { get; set; }

        /// <summary>
        /// The identifier post owner.
        /// </summary>
        public Guid PostedBy { get; set; }

        /// <summary>
        /// Date of post creation.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Date of post change.
        /// </summary>
        public DateTime? ChangedDate { get; set; }

        /// <summary>
        /// The url of the user's post.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Include Post | PostForward.
        /// </summary>
        public PostType PostType { get; set; }

        /// <summary>
        /// The identifier community.
        /// </summary>
        public Guid? CommunityId { get; set; }

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

        /// <summary>
        /// Type of post.
        /// </summary>
        public string Type { get; set; }
    }
}
