using System;
using System.Text.Json.Serialization;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Domain.ValueObject;

namespace Microservice.NewsFeed.Application.Consumers.Messages
{
    public class UserPostMessage
    {
        /// <summary>
        /// The identifier post.
        /// </summary>
        [JsonPropertyName("id")]
        public int PostId { get; set; }

        /// <summary>
        /// Considered as post content.
        /// </summary>
        [JsonPropertyName("message")]
        public string PostContent { get; set; }

        /// <summary>
        /// Post owner.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Post editor.
        /// </summary>
        public Guid UpdatedBy { get; set; }

        /// <summary>
        /// Include userWall, community.
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// Date of post creation.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///  Date of post change.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Post privacy.
        /// </summary>
        public string Visibility { get; set; }

        /// <summary>
        /// True if the post has forwarded from another user.
        /// </summary>
        public bool HasContentForward { get; set; }

        /// <summary>
        /// Include post, wiki, forum, poll.
        /// </summary>
        public string ContentForwardType { get; set; }

        /// <summary>
        /// The content of the post is forwarded.
        /// </summary>
        public UserPostMessage ContentForward { get; set; }

        public Source Source { get; set; }

        public PostType PostType
        {
            get
            {
                return HasContentForward
                    ? PostType.PostForward
                    : PostType.Post;
            }
        }

        public bool IsPostForward()
        {
            return HasContentForward && ContentForwardType == Post.PostTypeForward;
        }

        /// <summary>
        /// Check the post by user self-posted.
        /// </summary>
        /// <returns>Returns true if the post by user self-posted,
        /// otherwise false.</returns>
        public bool IsUserSelfPosted()
        {
            return CreatedBy == Source.UserId;
        }

        /// <summary>
        /// Check the post is public or private.
        /// </summary>
        /// <returns>
        /// Returns true if the Visibility is equal to <see cref="Post.PublicMode"/>,
        /// otherwise false.</returns>
        public bool IsPostPublic()
        {
            return Visibility == Post.PublicMode || Visibility == null;
        }

        /// <summary>
        /// Check the post forwarded from the post on community wall.
        /// </summary>
        /// <returns>
        /// Returns true if the post is forwarded from community,
        /// otherwise false.</returns>
        public bool IsPostForwardedFromCommunity()
        {
            return IsPostForward() && ContentForward.SourceType == Post.FromCommunity;
        }

        /// <summary>
        /// Check the post forwarded from the post on user wall.
        /// </summary>
        /// <returns>Returns true if the post is forwarded from user wall,
        /// otherwise false.</returns>
        public bool IsPostForwardedFromUserPost()
        {
            return IsPostForward() && ContentForward.SourceType == Post.FromUserWall;
        }

        public bool IsPostFromUserWall()
        {
            return ContentForward.SourceType == Post.FromUserWall;
        }
    }
}
