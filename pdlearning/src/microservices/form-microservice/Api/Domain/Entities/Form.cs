using System;
using System.Diagnostics.CodeAnalysis;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Versioning.Core;
using Microservice.Form.Versioning.Entities;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Domain.Entities
{
    [SchemaVersion("1.0", VersionSchemaType.Form)]
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class Form : BaseEntity, ISoftDelete, IVersioningFields, IHasDepartment
    {
        public static readonly int MaxTitleLength = 1000;

        public string Title { get; set; } = string.Empty;

        public FormType Type { get; set; }

        public FormStatus Status { get; set; }

        public FormSurveyType? SurveyType { get; set; }

        public bool? IsSurveyTemplate { get; set; }

        public Guid OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DueDate { get; set; }

        public int? InSecondTimeLimit { get; set; }

        public bool RandomizedQuestions { get; set; }

        public short? MaxAttempt { get; set; }

        public short? PassingMarkPercentage { get; set; }

        public int? PassingMarkScore { get; set; }

        public bool? ShowQuizSummary { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public bool? IsAllowedDisplayPollResult { get; set; }

        public Guid OriginalObjectId { get; set; }

        public Guid ParentId { get; set; }

        public bool IsArchived { get; set; }

        public int DepartmentId { get; set; }

        public bool? IsShowFreeTextQuestionInPoll { get; set; }

        public short? AttemptToShowFeedback { get; set; }

        public AnswerFeedbackDisplayOption? AnswerFeedbackDisplayOption { get; set; }

        public SqRatingType? SqRatingType { get; set; }

        public DateTime? SubmitDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        public FormStandaloneMode? StandaloneMode { get; set; }

        public bool? IsStandalone { get; set; }

        public DateTime? FormRemindDueDate { get; set; }

        public int RemindBeforeDays { get; set; }

        public bool IsSendNotification { get; set; }

        public bool? PublishToCatalogue { get; set; }
    }
}
