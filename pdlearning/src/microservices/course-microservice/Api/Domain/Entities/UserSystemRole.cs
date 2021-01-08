using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Course.Domain.Entities
{
    public class UserSystemRole : FullAuditedEntity
    {
        public Guid UserId { get; set; }

        public string Value { get; set; }

        public virtual CourseUser CourseUser { get; set; }

        public static UserSystemRole Create(Guid userId, string value)
        {
            return new UserSystemRole()
            {
                UserId = userId,
                Value = value
            };
        }
    }
}
