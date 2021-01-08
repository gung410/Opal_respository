using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.StandaloneSurvey.Domain
{
    public abstract class BaseEntity : AuditedEntity
    {
        /// <summary>
        ///  This property is needed for data migration and tracking data from opal 1.0 to opal 2.0.
        ///  It will store the old Ids of old entities to debug in case any issue later.
        ///  Definitely, will consider to remove it when the system is kind of stable!.
        /// </summary>
        public string ExternalId { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }
    }
}
