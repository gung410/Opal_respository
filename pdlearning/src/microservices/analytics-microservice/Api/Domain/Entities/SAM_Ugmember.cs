using System;

namespace Microservice.Analytics.Domain.Entities
{
    public class SAM_Ugmember
    {
        public long UgmemberId { get; set; }

        public int UserGroupId { get; set; }

        public Guid? UserId { get; set; }

        public DateTime? Created { get; set; }

        public Guid? CreatedByUserId { get; set; }

        public int? MemberRoleId { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public byte[] EntityVersion { get; set; }

        public DateTime? LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime? LastSynchronized { get; set; }

        public int? EntityStatusId { get; set; }

        public int? EntityStatusReasonId { get; set; }

        public string ExtId { get; set; }

        public DateTime? Deleted { get; set; }

        public string DisplayName { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }
    }
}
