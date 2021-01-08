using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Calendar.Domain.Entities
{
    /// <summary>
    /// Holds the sharing accesses of Team Calendar.
    /// </summary>
    public class TeamAccessSharing : FullAuditedEntity
    {
        /// <summary>
        /// The ID of person who shares the access.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// The ID of person who receives the access.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
