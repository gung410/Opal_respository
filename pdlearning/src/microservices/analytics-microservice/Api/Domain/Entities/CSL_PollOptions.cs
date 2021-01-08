using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_PollOptions
    {
        public CSL_PollOptions()
        {
            CslPollAnswerUser = new HashSet<CSL_PollAnswerUser>();
        }

        public int Id { get; set; }

        public int PollId { get; set; }

        public string Answer { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid UpdatedByUserId { get; set; }

        public Guid UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public virtual CSL_Poll Poll { get; set; }

        public virtual ICollection<CSL_PollAnswerUser> CslPollAnswerUser { get; set; }
    }
}
