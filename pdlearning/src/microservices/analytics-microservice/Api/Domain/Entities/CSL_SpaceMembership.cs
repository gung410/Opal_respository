using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_SpaceMembership
    {
        public int Id { get; set; }

        public Guid? SpaceId { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public Guid? OriginatorUserId { get; set; }

        public Guid? OriginatorUserHistoryId { get; set; }

        public byte? Status { get; set; }

        public DateTime? LastVisitDate { get; set; }

        public DateTime? AssignedFromDate { get; set; }

        public DateTime? AssignedToDate { get; set; }

        public Guid? AssignedByUserId { get; set; }

        public Guid? AssignedByUserHistoryId { get; set; }

        public string AssignedByDepartmentId { get; set; }

        public string MembershipType { get; set; }

        public byte? ShowAtDashboard { get; set; }

        public virtual CSL_Space Space { get; set; }

        public virtual SAM_User User { get; set; }
    }
}
