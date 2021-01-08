using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_ForumThread : Entity<int>
    {
        public CSL_ForumThread()
        {
            CslCommentForumThread = new HashSet<CSL_CommentForumThread>();
            CslForwardForumThread = new HashSet<CSL_ForwardForumThread>();
            CslLikeForumThread = new HashSet<CSL_LikeForumThread>();
            CslUserFollowForumThread = new HashSet<CSL_UserFollowForumThread>();
            CslForumThreadRevision = new HashSet<CSL_ForumThreadRevision>();
        }

        public string Title { get; set; }

        public byte IsHome { get; set; }

        public byte AdminOnly { get; set; }

        public byte? IsCategory { get; set; }

        public int? ParentThreadId { get; set; }

        public int? SortOrder { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public Guid? SpaceId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid UpdatedByUserId { get; set; }

        public Guid UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual ICollection<CSL_CommentForumThread> CslCommentForumThread { get; set; }

        public virtual ICollection<CSL_ForwardForumThread> CslForwardForumThread { get; set; }

        public virtual ICollection<CSL_LikeForumThread> CslLikeForumThread { get; set; }

        public virtual ICollection<CSL_UserFollowForumThread> CslUserFollowForumThread { get; set; }

        public virtual ICollection<CSL_ForumThreadRevision> CslForumThreadRevision { get; set; }
    }
}
