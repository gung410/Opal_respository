using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Microservice.NewsFeed.Domain.Entities
{
    public class LearnerCourseSubscription
    {
        /// <summary>
        /// This field automatically is generated please don't create new for it.
        /// </summary>
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        /// <summary>
        /// List of user identification ids.
        /// </summary>
        public List<Guid> LearnerIds { get; set; }

        /// <summary>
        /// Course Id.
        /// </summary>
        public Guid CourseId { get; set; }
    }
}
