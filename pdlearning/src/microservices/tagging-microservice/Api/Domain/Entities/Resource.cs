using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable SA1201 // Elements should appear in the correct order
namespace Conexus.Opal.Microservice.Tagging.Domain.Entities
{
    public class Resource
    {
        public Guid ResourceId { get; set; }

        public ResourceType ResourceType { get; set; }

        public Guid? MainSubjectAreaTagId { get; set; }

        public string PreRequisties { get; set; }

        public string ObjectivesOutCome { get; set; }

        public Guid CreatedBy { get; set; }

        public IEnumerable<Guid> Tags { get; set; } = new List<Guid>();

        public List<string> SearchTags { get; set; } = new List<string>();

        public Dictionary<string, object> DynamicMetaData { get; set; } = new Dictionary<string, object>();

        public Resource Clone(Guid resourceId, Guid userId)
        {
            return new Resource
            {
                CreatedBy = userId,
                DynamicMetaData = DynamicMetaData,
                MainSubjectAreaTagId = MainSubjectAreaTagId,
                ObjectivesOutCome = ObjectivesOutCome,
                PreRequisties = PreRequisties,
                ResourceId = resourceId,
                ResourceType = ResourceType,
                Tags = Tags,
                SearchTags = SearchTags
            };
        }
    }

    public enum ResourceType
    {
        Content,
        Course,
        Community,
        LearningPath,
        Form
    }
}
#pragma warning restore SA1201 // Elements should appear in the correct order
