using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CCPM_FormParticipant
    {
        public Guid FormParticipantId { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public Guid UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public Guid FormId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string Status { get; set; }

        public bool IsDeleted { get; set; }

        public bool? IsStarted { get; set; }

        public virtual CCPM_Form Form { get; set; }
    }
}
