using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    /// <summary>
    /// Sync from Form table on the Form module.
    /// </summary>
    public class Form : AuditedEntity
    {
        public string Title { get; set; }

        public FormType Type { get; set; }

        public FormStatus Status { get; set; }

        public FormSurveyType? SurveyType { get; set; }

        public DateTime? DueDate { get; set; }

        public Guid OriginalObjectId { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public FormStandaloneMode? StandaloneMode { get; set; }

        public bool? IsStandalone { get; set; }

        public void Update(
            string title,
            FormType type,
            FormStatus status,
            FormSurveyType? surveyType,
            DateTime? dueDate,
            Guid originalObjectId,
            bool isArchived,
            DateTime? startDate,
            DateTime? endDate,
            FormStandaloneMode? standaloneMode,
            bool? isStandalone)
        {
            Title = title;
            Type = type;
            Status = status;
            SurveyType = surveyType;
            DueDate = dueDate;
            OriginalObjectId = originalObjectId;
            IsArchived = isArchived;
            StartDate = startDate;
            EndDate = endDate;
            StandaloneMode = standaloneMode;
            IsStandalone = isStandalone;
        }

        public bool IsStandaloneForm()
        {
            return IsStandalone.HasValue && IsStandalone.Value;
        }

        public bool IsStandaloneFormPublished()
        {
            return IsStandaloneForm() && Status == FormStatus.Published;
        }
    }
}
