using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    public class Comment : AuditedEntity
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// The form origial object Id.
        /// </summary>
        public Guid ObjectId { get; set; }

        public string Content { get; set; }
    }
}
