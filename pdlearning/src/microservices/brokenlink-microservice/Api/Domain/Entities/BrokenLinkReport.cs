using System;
using Conexus.Opal.BrokenLinkChecker;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.BrokenLink.Domain.Entities
{
    public class BrokenLinkReport : AuditedEntity
    {
        /// <summary>
        /// The ID of object, where object is the content contains the <see cref="Url"/>.
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// The object's url/link that is broken need to report.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The owner (module) of the <see cref="Url"/>.
        /// </summary>
        public ModuleIdentifier Module { get; set; }

        /// <summary>
        /// Describe invalid reason of <see cref="Url"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// User ID. Equals <b>null</b> if the report was reported by system.
        /// </summary>
        public Guid? ReportBy { get; set; }

        /// <summary>
        /// The ReportBy'fullname.
        /// </summary>
        public string ReporterName { get; set; }

        /// <summary>
        /// <b>True</b> if the report was reported by system schedule.
        /// </summary>
        public bool IsSystemReport { get; set; }

        /// <summary>
        /// Used to support multiple version feature of the object.
        /// </summary>
        public Guid? OriginalObjectId { get; set; }

        /// <summary>
        /// Used to support CAM to get reports of course from class run.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// The name of object like digital content title, course name...
        /// </summary>
        public string ObjectTitle { get; set; }

        /// <summary>
        /// The owner's ID of object.
        /// </summary>
        public Guid ObjectOwnerId { get; set; }

        /// <summary>
        /// The owner's full name of object.
        /// </summary>
        public string ObjectOwnerName { get; set; }

        /// <summary>
        /// The deep link to the object.
        /// </summary>
        public string ObjectDetailUrl { get; set; }

        /// <summary>
        /// The deep link to the object.
        /// </summary>
        public BrokenLinkContentType ContentType { get; set; }
    }
}
