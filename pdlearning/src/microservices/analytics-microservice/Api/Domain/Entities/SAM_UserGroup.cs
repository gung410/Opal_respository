using System;

namespace Microservice.Analytics.Domain.Entities
{
    public class SAM_UserGroup
    {
        public int UserGroupId { get; set; }

        public string DepartmentId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public string ExtId { get; set; }

        public int? UserId { get; set; }

        public int? UserGroupTypeId { get; set; }

        public string Tag { get; set; }

        public byte[] EntityVersion { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime LastSynchronized { get; set; }

        public string ArchetypeId { get; set; }

        public DateTime? Deleted { get; set; }

        public int? EntityStatusId { get; set; }

        public int? EntityStatusReasonId { get; set; }
    }
}
