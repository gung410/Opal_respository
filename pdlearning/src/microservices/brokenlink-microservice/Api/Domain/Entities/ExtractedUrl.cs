using System;
using System.ComponentModel.DataAnnotations;
using Conexus.Opal.BrokenLinkChecker;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.BrokenLink.Domain.Entities
{
    public enum ScanUrlStatus
    {
        None,
        Valid,
        Invalid,
        Checking
    }

    public class ExtractedUrl : AuditedEntity
    {
        public string Url { get; set; }

        /// <summary>
        /// The owner (module) of the <see cref="Url"/>.
        /// </summary>
        public ModuleIdentifier Module { get; set; }

        [ConcurrencyCheck]
        public ScanUrlStatus Status { get; set; } = ScanUrlStatus.None;

        /// <summary>
        /// The ID of object, where object is the content contains the <see cref="Url"/>.
        /// </summary>
        public Guid ObjectId { get; set; }

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
        /// The owner's full name of object.
        /// </summary>
        public string ObjectOwnerName { get; set; }

        /// <summary>
        /// The owner's ID of object.
        /// </summary>
        public Guid ObjectOwnerId { get; set; }

        /// <summary>
        /// The deep link to the object.
        /// </summary>
        public string ObjectDetailUrl { get; set; }

        public DateTime? ScannedAt { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public BrokenLinkContentType ContentType { get; set; }
    }
}
