using System.Collections.Generic;
using Conexus.Opal.Microservice.Metadata.Entities;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;

namespace Conexus.Opal.Microservice.Tagging.Dtos
{
    public class GetResourceWithMetadataResult
    {
        public Resource Resource { get; set; }

        public IEnumerable<MetadataTag> MetadataTags { get; set; }
    }
}
