using MongoDB.Bson.Serialization.Attributes;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Badge.Domain.Entities
{
    public class UserEntity : AuditedEntity, ISoftDelete
    {
        /// <summary>
        /// Used to map user ID from original database.
        /// </summary>
        [BsonElement("UserId")]
        public int OriginalUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public bool IsDeleted { get; set; }

        public string FullName()
        {
            return (FirstName ?? string.Empty) + " " + (LastName ?? string.Empty).Trim();
        }
    }
}
