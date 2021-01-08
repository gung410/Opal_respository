using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_AttendanceTracking
    {
        public Guid AttendanceTrackingId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid SessionId { get; set; }

        public Guid RegistrationId { get; set; }

        public Guid Userid { get; set; }

        public Guid? UserHistoryId { get; set; }

        public Guid? DepartmentId { get; set; }

        public string ReasonForAbsence { get; set; }

        public string Status { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? CodeScannedDate { get; set; }

        public bool IsCodeScanned { get; set; }

        public string Attachment { get; set; }

        public virtual CAM_Registration Registration { get; set; }

        public virtual CAM_Session Session { get; set; }
    }
}
