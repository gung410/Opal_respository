using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_Session
    {
        public CAM_Session()
        {
            CamAttendanceTracking = new HashSet<CAM_AttendanceTracking>();
        }

        public Guid SessionId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid ClassRunId { get; set; }

        public bool IsDeleted { get; set; }

        public string ExternalId { get; set; }

        public Guid? ChangedByUserId { get; set; }

        public Guid? ChangedByUserHistoryId { get; set; }

        public string ChangedByDepartmentId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string SessionTitle { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? RescheduleEndDateTime { get; set; }

        public DateTime? RescheduleStartDateTime { get; set; }

        public string Venue { get; set; }

        public string SessionCode { get; set; }

        public virtual CAM_ClassRun ClassRun { get; set; }

        public virtual ICollection<CAM_AttendanceTracking> CamAttendanceTracking { get; set; }
    }
}
