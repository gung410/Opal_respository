using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Form.Domain.Entities
{
    public class AccessRight : AuditedEntity
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// The digital content origial object Id.
        /// </summary>
        public Guid ObjectId { get; set; }

        public Guid CreatedBy { get; set; }
    }
}
