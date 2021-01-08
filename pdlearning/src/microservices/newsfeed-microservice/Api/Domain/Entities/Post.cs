using System;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Microservice.NewsFeed.Domain.Entities
{
    /// <summary>
    /// Sync from Post of the CSL module.
    /// </summary>
    public class Post
    {
        public const string FromCommunity = "community";
        public const string FromUserWall = "userWall";
        public const string PrivateMode = "private";
        public const string PublicMode = "public";
        public const string PostTypeForward = "post";
        public const string WikiTypeForward = "wiki";
        public const string ForumTypeForward = "forum";
        public const string PollTypeForward = "poll";

        public Post(
            int postId,
            string postContent,
            string url,
            string sourceType,
            string visibility,
            Guid createdBy,
            Guid? changedBy,
            DateTime createdDate,
            DateTime? changedDate)
        {
            PostId = postId;
            PostContent = postContent;
            Url = url;
            SourceType = sourceType;
            Visibility = visibility;
            CreatedBy = createdBy;
            ChangedBy = changedBy;
            CreatedDate = createdDate;
            ChangedDate = changedDate;
        }

        /// <summary>
        /// This field automatically is generated please don't create new for it.
        /// </summary>
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        /// <summary>
        /// The identifier post.
        /// </summary>
        public int PostId { get; private set; }

        /// <summary>
        /// The content of the post.
        /// </summary>
        public string PostContent { get; private set; }

        /// <summary>
        /// Include userWall | community.
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// Post owner's url.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Include Post | PostForward.
        /// </summary>
        public ValueObject.PostType PostType { get; private set; }

        /// <summary>
        /// Include Private, Public, Owner.
        /// </summary>
        public string Visibility { get; private set; }

        /// <summary>
        /// The identifier post owner.
        /// </summary>
        public Guid CreatedBy { get; private set; }

        /// <summary>
        /// The identifier post owner.
        /// </summary>
        public Guid? ChangedBy { get; private set; }

        /// <summary>
        /// Date of post creation.
        /// </summary>
        public DateTime CreatedDate { get; private set; }

        /// <summary>
        /// Date of post change.
        /// </summary>
        public DateTime? ChangedDate { get; private set; }

        public static Expression<Func<Post, bool>> FilterByPostId(int postId)
        {
            return p => p.PostId == postId;
        }

        public void Update(
            string postContent,
            string url,
            string sourceType,
            string visibility,
            Guid? changedBy,
            DateTime? changedDate)
        {
            PostContent = postContent;
            Url = url;
            SourceType = sourceType;
            Visibility = visibility;
            ChangedBy = changedBy;
            ChangedDate = changedDate;
        }

        public void SetPostType(ValueObject.PostType postType)
        {
            PostType = postType;
        }

        /// <summary>
        /// Check post has been made public.
        /// </summary>
        /// <returns>Returns true if the Visibility is equal to <see cref="PublicMode"/>,
        /// otherwise false.</returns>
        public bool IsPostPublic()
        {
            return Visibility == PublicMode;
        }
    }
}
