using System.Collections.Generic;
using Microservice.Course.Domain.ValueObjects;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Course.Domain.Entities
{
    public class ECertificateLayout : FullAuditedEntity, ISoftDelete
    {
        public string Name { get; set; }

        public string LayoutFileName { get; set; }

        public string Description { get; set; }

        public IEnumerable<ECertificateLayoutParam> Params { get; set; }

        public bool IsDeleted { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string PreviewImagePath { get; set; }

        public bool IsSystem { get; set; }
    }
}
