using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_WikiPage : Entity<int>
    {
        public CSL_WikiPage()
        {
            CslCommentWikiPage = new HashSet<CSL_CommentWikiPage>();
            CslForwardWikiPage = new HashSet<CSL_ForwardWikiPage>();
            CslLikeWikiPage = new HashSet<CSL_LikeWikiPage>();
            CslUserFollowWikiPage = new HashSet<CSL_UserFollowWikiPage>();
            CslWikiPageRevision = new HashSet<CSL_WikiPageRevision>();
        }

        public string Title { get; set; }

        public byte IsHome { get; set; }

        public byte AdminOnly { get; set; }

        public byte? IsCategory { get; set; }

        public int? ParentPageId { get; set; }

        public int? SortOrder { get; set; }

        public Guid? SpaceId { get; set; }

        public string Content { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid UpdatedByUserId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public Guid UpdatedByUserHistoryId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual ICollection<CSL_CommentWikiPage> CslCommentWikiPage { get; set; }

        public virtual ICollection<CSL_ForwardWikiPage> CslForwardWikiPage { get; set; }

        public virtual ICollection<CSL_LikeWikiPage> CslLikeWikiPage { get; set; }

        public virtual ICollection<CSL_UserFollowWikiPage> CslUserFollowWikiPage { get; set; }

        public virtual ICollection<CSL_WikiPageRevision> CslWikiPageRevision { get; set; }
    }
}
