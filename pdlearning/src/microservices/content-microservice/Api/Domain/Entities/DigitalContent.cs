using System;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Domain.Enums;
using Microservice.Content.Versioning.Core;
using Microservice.Content.Versioning.Entities;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Content.Domain.Entities
{
    [SchemaVersion("1.0", VersionSchemaType.DigitalContent)]
    public class DigitalContent : AuditedEntity, ISoftDelete, IVersioningFields, IHasDepartment
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public ContentType Type { get; set; }

        public DigitalContentStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public Guid ChangedBy { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid OwnerId { get; set; }

        public string ExternalId { get; set; }

        public string RepositoryName { get; set; }

        // Copyright section
        public string Source { get; set; }

        public Ownership Ownership { get; set; }

        public LicenseType LicenseType { get; set; }

        public string TermsOfUse { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public LicenseTerritory LicenseTerritory { get; set; }

        public string Publisher { get; set; }

        public string Copyright { get; set; }

        public bool IsAllowReusable { get; set; }

        public bool IsAllowDownload { get; set; }

        public bool IsAllowModification { get; set; }

        public string AcknowledgementAndCredit { get; set; }

        public string Remarks { get; set; }

        public Guid OriginalObjectId { get; set; }

        public Guid ParentId { get; set; }

        public bool IsArchived { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public int DepartmentId { get; set; }

        public double AverageRating { get; set; } = 0;

        public int ReviewCount { get; set; } = 0;

        public DateTime? SubmitDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        public bool? IsAutoPublish { get; set; }

        public DateTime? AutoPublishDate { get; set; }
    }
}
