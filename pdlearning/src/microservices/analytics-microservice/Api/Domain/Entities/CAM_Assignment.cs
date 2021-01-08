using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public class CAM_Assignment
    {
        public CAM_Assignment()
        {
            LearnerUserAssignments = new HashSet<Learner_UserAssignment>();
        }

        public Guid AssignmentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public Guid? ChangedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public string ChangedByDepartmentId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartDate { get; set; }

        public virtual CAM_ClassRun ClassRun { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual ICollection<Learner_UserAssignment> LearnerUserAssignments { get; set; }
    }
}
