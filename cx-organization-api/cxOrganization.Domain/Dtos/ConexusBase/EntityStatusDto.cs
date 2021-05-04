using System;
using cxOrganization.Domain.Attributes.CustomValidationAttributes;

namespace cxPlatform.Client.ConexusBase
{
    public class EntityStatusDto
    {
        public bool ExternallyMastered { get; set; }
        public DateTime? LastExternallySynchronized { get; set; }

        [CurrentDate(ErrorMessage = "Expiration date must be after or equal to current date")]
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
