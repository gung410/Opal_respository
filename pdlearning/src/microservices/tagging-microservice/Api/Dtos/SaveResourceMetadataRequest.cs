using System;
using System.Collections.Generic;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;

namespace Conexus.Opal.Microservice.Tagging.Dtos
{
    public class SaveResourceMetadataRequest
    {
        public IEnumerable<Guid> TagIds { get; set; }

        public Dictionary<string, object> DynamicMetaData { get; set; }

        public Guid? MainSubjectAreaTagId { get; set; }

        public string PreRequisties { get; set; }

        public string ObjectivesOutCome { get; set; }

        public List<string> SearchTags { get; set; }
    }
}
