using System;
using Microservice.NewsFeed.Domain.ValueObject;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservice.NewsFeed.Domain.Entities
{
    [BsonKnownTypes(typeof(CommunityPostFeed))]
    public class UserPostFeed : NewsFeed
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
        /// Date of newsfeed change.
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
        /// The content of the post is forwarded.
        /// </summary>
        public UserPostForward PostForward { get; set; }

        /// <summary>
        /// Type of post.
        /// </summary>
        public override string Type => nameof(UserPostFeed);

        public void AddPostForward(UserPostForward postForward)
        {
            PostForward = postForward;
        }

        public void AddUserId(Guid userId)
        {
            UserId = userId;
        }
    }
}
