using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_PollAnswerUser
    {
        public int Id { get; set; }

        public int PollId { get; set; }

        public int PollOptionId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public virtual CSL_Poll Poll { get; set; }

        public virtual CSL_PollOptions PollOption { get; set; }
    }
}
