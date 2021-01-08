using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CoursePlanningCycle
    {
        public CAM_CoursePlanningCycle()
        {
            CamCourse = new HashSet<CAM_Course>();
        }

        public Guid CoursePlanningCycleId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string ExternalId { get; set; }

        public int YearCycle { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public Guid? ChangedByUserHistoryId { get; set; }

        public string ChangedByDepartmentId { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<CAM_Course> CamCourse { get; set; }
    }
}
