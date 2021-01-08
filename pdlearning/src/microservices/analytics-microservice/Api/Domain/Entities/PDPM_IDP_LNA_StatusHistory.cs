using System;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class PDPM_IDP_LNA_StatusHistory : Entity<Guid>
    {
        public Guid IDP_LNA_Id { get; set; }

        public int StatusTypeId { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public Guid? ByUserId { get; set; }

        public long ResultId { get; set; }
    }
}
