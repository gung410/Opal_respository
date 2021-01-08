using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_Post : Entity<int>
    {
        public CSL_Post()
        {
            CslCommentPost = new HashSet<CSL_CommentPost>();
            CslForwardPost = new HashSet<CSL_ForwardPost>();
            CslLikePost = new HashSet<CSL_LikePost>();
            CslUserFollowPost = new HashSet<CSL_UserFollowPost>();
        }

        public string Message { get; set; }

        public string Url { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid UpdatedByUserId { get; set; }

        public Guid UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public Guid? SpaceId { get; set; }

        public Guid? UserWallId { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual ICollection<CSL_CommentPost> CslCommentPost { get; set; }

        public virtual ICollection<CSL_ForwardPost> CslForwardPost { get; set; }

        public virtual ICollection<CSL_LikePost> CslLikePost { get; set; }

        public virtual ICollection<CSL_UserFollowPost> CslUserFollowPost { get; set; }
    }
}
