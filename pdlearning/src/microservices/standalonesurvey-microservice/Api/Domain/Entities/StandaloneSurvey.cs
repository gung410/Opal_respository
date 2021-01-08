using System;
using System.Diagnostics.CodeAnalysis;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microservice.StandaloneSurvey.Versioning.Core;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    [SchemaVersion("1.0", VersionSchemaType.StandaloneSurvey)]
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class StandaloneSurvey : BaseEntity, ISoftDelete, IVersioningFields, IHasDepartment
    {
        public static readonly int MaxTitleLength = 1000;

        public string Title { get; set; } = string.Empty;

        public SurveyStatus Status { get; set; }

        public Guid OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DueDate { get; set; }

        public Guid OriginalObjectId { get; set; }

        public Guid ParentId { get; set; }

        public bool IsArchived { get; set; }

        public int DepartmentId { get; set; }

        /// <summary>
        /// Only with CSL communityId will have value.
        /// Otherwise it will be null value for LNA.
        /// </summary>
        public Guid? CommunityId { get; set; }

        public SqRatingType? SqRatingType { get; set; }

        public DateTime? SubmitDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        public DateTime? FormRemindDueDate { get; set; }

        public int RemindBeforeDays { get; set; }
    }
}
