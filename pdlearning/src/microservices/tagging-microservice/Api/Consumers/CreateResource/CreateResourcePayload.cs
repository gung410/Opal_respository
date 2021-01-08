using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;

namespace Conexus.Opal.Microservice.Tagging.Consumers.CreateResource
{
    public class CreateResourcePayload
    {
        public Guid Id { get; set; }

        public ResourceType ResourceType { get; set; }

        public Guid? MainSubjectAreaTagId { get; set; }

        public string PreRequisties { get; set; }

        public string ObjectivesOutCome { get; set; }

        public Guid CreatedBy { get; set; }

        public IEnumerable<Guid> Tags { get; set; } = new List<Guid>();

        public IEnumerable<string> SearchTags { get; set; }

        public Dictionary<string, object> DynamicMetaData { get; set; } = new Dictionary<string, object>();

        public ResourceDictionary Dictionary { get; set; }
    }

    public class ResourceDictionary
    {
        public Dictionary<string, object> Dictionary { get; set; } = new Dictionary<string, object>();
    }
}
