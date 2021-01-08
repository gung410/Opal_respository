using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_Group
    {
        public CSL_Group()
        {
            CslGroupPermission = new HashSet<CSL_GroupPermission>();
            CslGroupUser = new HashSet<CSL_GroupUser>();
        }

        public int Id { get; set; }

        public int? SpaceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid UpdatedByUserId { get; set; }

        public Guid UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public byte IsAdminGroup { get; set; }

        public byte ShowAtRegistration { get; set; }

        public virtual ICollection<CSL_GroupPermission> CslGroupPermission { get; set; }

        public virtual ICollection<CSL_GroupUser> CslGroupUser { get; set; }
    }
}
