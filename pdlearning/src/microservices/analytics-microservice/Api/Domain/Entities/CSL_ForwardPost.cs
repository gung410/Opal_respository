using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_ForwardPost
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int? PostIdForwarded { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public virtual CSL_Post Post { get; set; }
    }
}
