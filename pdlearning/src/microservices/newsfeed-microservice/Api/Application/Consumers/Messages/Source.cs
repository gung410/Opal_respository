using System;
using System.Text.Json.Serialization;

namespace Microservice.NewsFeed.Application.Consumers.Messages
{
    public class Source
    {
        /// <summary>
        /// <see cref="int"/> if the post is posted on the user wall,
        /// <see cref="System.Guid"/> if post is posted on the community.
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        /// Posts by user self-posted on wall => Post owner Id
        /// otherwise the user id is forwarded.
        /// </summary>
        [JsonPropertyName("guid")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// User or community url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Name of community.
        /// </summary>
        [JsonPropertyName("name")]
        public string CommunityName { get; set; }

        /// <summary>
        /// Description of community.
        /// </summary>
        [JsonPropertyName("description")]
        public string CommunityDescription { get; set; }

        /// <summary>
        /// Thumbnail url of community.
        /// </summary>
        public string CommunityThumbnailUrl { get; set; }

        /// <summary>
        /// The identifier community.
        /// </summary>
        public Guid? MainCommunityId { get; set; }

        /// <summary>
        /// Status of user or community.
        /// </summary>
        public string Status { get; set; }

        public string Visibility { get; set; }
    }
}
