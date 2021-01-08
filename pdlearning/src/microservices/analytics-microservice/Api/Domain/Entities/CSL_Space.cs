using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_Space : Entity<Guid>
    {
        public CSL_Space()
        {
            CslSpaceMembership = new HashSet<CSL_SpaceMembership>();
            CslUserFollowSpace = new HashSet<CSL_UserFollowSpace>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte? JoinPolicy { get; set; }

        public byte? Visibility { get; set; }

        public byte Status { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? CreatedByUserId { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? UpdatedByUserId { get; set; }

        public Guid? UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public string Url { get; set; }

        public Guid? SpaceParentId { get; set; }

        public string CoursesId { get; set; }

        public string ClassRunId { get; set; }

        public string Tags { get; set; }

        public Guid? OwnerId { get; set; }

        public virtual ICollection<CSL_SpaceMembership> CslSpaceMembership { get; set; }

        public virtual ICollection<CSL_UserFollowSpace> CslUserFollowSpace { get; set; }
    }
}
