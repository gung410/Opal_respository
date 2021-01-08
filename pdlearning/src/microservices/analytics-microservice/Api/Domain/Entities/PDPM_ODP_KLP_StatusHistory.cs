using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public class PDPM_ODP_KLP_StatusHistory : Entity<Guid>
    {
        public Guid ODP_KLP_Id { get; set; }

        public int StatusTypeId { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public Guid? ByUserId { get; set; }

        public long ResultId { get; set; }
    }
}
