using System;
using System.Collections.Generic;
using System.Linq;
using Conexus.Opal.Microservice.Metadata.Entities;
using Conexus.Opal.Microservice.Tagging.Models;

namespace Conexus.Opal.Microservice.Tagging.Dtos
{
    public class GetResourceMetadataListResult
    {
        public GetResourceMetadataListResult()
        {
        }

        public IEnumerable<ResourceMetadatasModel> Items { get; set; }
    }
}
