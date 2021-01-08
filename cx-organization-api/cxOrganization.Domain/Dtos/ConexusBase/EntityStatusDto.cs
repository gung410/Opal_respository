using System;

namespace cxPlatform.Client.ConexusBase
{
    public class EntityStatusDto
    {
        public bool ExternallyMastered { get; set; }
        public DateTime? LastExternallySynchronized { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ActiveDate { get; set; }
        public byte[] EntityVersion { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedBy { get; set; }

        public EntityStatusEnum StatusId { get; set; }
        public EntityStatusReasonEnum StatusReasonId { get; set; }
        public bool Deleted { get; set; }
    }
}
