using System;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Course.Domain.Entities
{
    public class UserMetadata : FullAuditedEntity
    {
        public Guid UserId { get; set; }

        public UserMetadataValueType Type { get; set; }

        public string Value { get; set; }

        public virtual CourseUser CourseUser { get; set; }

        public static UserMetadata Create(Guid userId, UserMetadataValueType userMetadataValueType, string value)
        {
            return new UserMetadata()
            {
                UserId = userId,
                Type = userMetadataValueType,
                Value = value
            };
        }
    }
}
