using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.Entities;

namespace Conexus.Opal.Microservice.Tagging.Models
{
    public class ResourceMetadatasModel
    {
        public Guid ResourceId { get; set; }

        public IEnumerable<MetadataTag> Metadatas { get; set; }
    }
}
