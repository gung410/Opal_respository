using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_GroupUser
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public int GroupId { get; set; }

        public bool IsGroupManager { get; set; }

        public DateTime? AssignedFromDate { get; set; }

        public DateTime? AssignedToDate { get; set; }

        public Guid AssignedByUserId { get; set; }

        public Guid AssignedByUserHistoryId { get; set; }

        public string AssignedByDepartmentId { get; set; }

        public virtual CSL_Group Group { get; set; }

        public virtual SAM_User User { get; set; }
    }
}
