using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_Poll : Entity<int>
    {
        public CSL_Poll()
        {
            CslCommentPoll = new HashSet<CSL_CommentPoll>();
            CslLikePoll = new HashSet<CSL_LikePoll>();
            CslPollAnswerUser = new HashSet<CSL_PollAnswerUser>();
            CslPollOptions = new HashSet<CSL_PollOptions>();
            CslUserFollowPoll = new HashSet<CSL_UserFollowPoll>();
        }

        public string Question { get; set; }

        public short AllowMultiple { get; set; }

        public bool? Closed { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid UpdatedByUserId { get; set; }

        public Guid UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public Guid? SpaceId { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual ICollection<CSL_CommentPoll> CslCommentPoll { get; set; }

        public virtual ICollection<CSL_LikePoll> CslLikePoll { get; set; }

        public virtual ICollection<CSL_PollAnswerUser> CslPollAnswerUser { get; set; }

        public virtual ICollection<CSL_PollOptions> CslPollOptions { get; set; }

        public virtual ICollection<CSL_UserFollowPoll> CslUserFollowPoll { get; set; }
    }
}
