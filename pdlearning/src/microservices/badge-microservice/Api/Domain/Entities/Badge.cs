using System;
using System.Collections.Generic;
using Microservice.Badge.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Microservice.Badge.Domain.Entities
{
    [BsonIgnoreExtraElements] // Ignore extra element to avoid exception in case migrate badge data.
    public class BadgeEntity
    {
        public BadgeEntity(Guid id, BadgeType type)
        {
            Id = id;
            Type = type;
        }

        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; init; }

        public BadgeType Type { get; init; }

        public string Name { get; set; }

        /// <summary>
        /// List of level with relative path to S3 file.
        /// Eg: { Level1 : "/permanent/digital-badging/community-builder-lv1.png." }.
        /// </summary>
        public Dictionary<BadgeLevelEnum, string> LevelImages { get; set; }

        public string TagImage { get; set; }

        /// <summary>
        /// Increase version in case you want to upgrade badge information.
        /// </summary>
        public int SeedVersion { get; set; } = 1;
    }
}
