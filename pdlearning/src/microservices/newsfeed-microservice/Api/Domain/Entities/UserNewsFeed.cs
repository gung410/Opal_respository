using System;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Microservice.NewsFeed.Domain.Entities
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(PdpmSuggestCourseFeed), typeof(UserPostFeed))]
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class NewsFeed
    {
        protected NewsFeed()
        {
        }

        /// <summary>
        /// This field automatically is generated please don't create new for it.
        /// </summary>
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        /// <summary>
        /// The Id of user course subscription
        /// or the user is forwarded the post
        /// or followers.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Date of newsfeed creation.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        public virtual string Type => nameof(NewsFeed);
    }
}
