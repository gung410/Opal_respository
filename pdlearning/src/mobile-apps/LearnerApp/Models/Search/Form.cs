using System;

namespace LearnerApp.Models.Search
{
    public class Form
    {
        public string Title { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public bool RandomizedQuestions { get; set; }

        public string PrimaryApprovingOfficerId { get; set; }

        public string OriginalObjectId { get; set; }

        public string ParentId { get; set; }

        public bool IsArchived { get; set; }

        public int DepartmentId { get; set; }

        public string AnswerFeedbackDisplayOption { get; set; }

        public DateTime SubmitDate { get; set; }

        public string StandaloneMode { get; set; }

        public bool IsStandalone { get; set; }

        public int RemindBeforeDays { get; set; }

        public bool IsSendNotification { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Id { get; set; }
    }
}
