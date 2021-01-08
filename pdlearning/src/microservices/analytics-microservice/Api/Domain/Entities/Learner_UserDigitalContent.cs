using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserDigitalContent
    {
        public Learner_UserDigitalContent()
        {
            LearnerUserLearningPackages = new HashSet<Learner_UserLearningPackage>();
        }

        public Guid UserDigitalContentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public Guid DigitalContentId { get; set; }

        public string DepartmentId { get; set; }

        public string Status { get; set; }

        public string DigitalContentType { get; set; }

        public string Version { get; set; }

        public string ReviewStatus { get; set; }

        public double? ProgressMeasure { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DisenrollUtc { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime? ReminderSentDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public double? Rate { get; set; }

        public DateTime? RateDate { get; set; }

        public string RateCommentTitle { get; set; }

        public string RateComment { get; set; }

        public virtual SAM_Department Department { get; set; }

        public virtual CCPM_DigitalContent DigitalContent { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }

        public virtual ICollection<Learner_UserLearningPackage> LearnerUserLearningPackages { get; set; }
    }
}
