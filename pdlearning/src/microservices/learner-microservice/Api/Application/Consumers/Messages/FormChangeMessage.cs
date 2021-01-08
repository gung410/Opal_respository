using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Consumers
{
    public class FormChangeMessage
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public FormType Type { get; set; }

        public FormStatus Status { get; set; }

        public FormSurveyType? SurveyType { get; set; }

        public DateTime? DueDate { get; set; }

        public Guid OriginalObjectId { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public FormStandaloneMode? StandaloneMode { get; set; }

        public bool? IsStandalone { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public bool IsStandaloneForm()
        {
            return IsStandalone.HasValue && IsStandalone.Value;
        }

        public bool IsArchivedForm()
        {
            return Status == FormStatus.Archived;
        }

        public bool IsStandaloneFormPublished()
        {
            return IsStandaloneForm() && Status == FormStatus.Published;
        }

        public bool IsArchivedStandaloneVersionForm()
        {
            return IsStandaloneForm() && IsArchived;
        }
    }
}
