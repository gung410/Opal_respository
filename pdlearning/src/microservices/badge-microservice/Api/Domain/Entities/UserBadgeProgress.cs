using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Microservice.Badge.Domain.Entities
{
    /// <summary>
    /// This entity is represent information for a user Qualified For condition to still be in top after achieving badge from a specific day.
    /// If user break their top chain in specific Badge, we remove corresponding record.
    /// </summary>
    public class UserBadgeProgress
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public Guid BadgeId { get; init; }

        public DateTime StartDate { get; set; }
    }
}
