using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Course.Domain.Entities
{
    public class Comment : AuditedEntity
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// The entity id related to the comments. Example: CourseId, RegistrationId, etc ...
        /// </summary>
        public Guid ObjectId { get; set; }

        public string Content { get; set; }

        /// <summary>
        /// Define the comment for which type of action. Follow CommentActionConstant to see example.
        /// </summary>
        public string Action { get; set; }
    }
}
