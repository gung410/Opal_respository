using System;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Uploader.Domain.Entities
{
    public class PersonalSpace : AuditedEntity
    {
        public Guid UserId { get; set; }

        public double TotalSpace { get; set; }

        public double TotalUsed { get; set; }

        public bool IsStorageUnlimited { get; set; }
    }
}
