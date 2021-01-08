using System;

namespace Microservice.NewsFeed.Application.Consumers.Messages
{
    public class CommunityMessage
    {
        /// <summary>
        /// The identifier community.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the community.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the community.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thumbnail of the community.
        /// </summary>
        public string CommunityThumbnail { get; set; }

        /// <summary>
        /// Community url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Include archived | enabled.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The identifier community.
        /// </summary>
        public Guid? MainCommunityId { get; set; }

        /// <summary>
        /// The identifier community owner.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// The identifier community owner.
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// Date of community creation.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date of community change.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        public string Visibility { get; set; }
    }
}
