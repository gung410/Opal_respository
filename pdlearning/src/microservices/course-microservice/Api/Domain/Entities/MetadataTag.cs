using System;
using Conexus.Opal.Microservice.Metadata.Entities;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Course.Domain.Entities
{
    public class MetadataTag : Entity, IMetadataTag
    {
        public Guid TagId
        {
            get
            {
                return Id;
            }

            set
            {
                Id = value;
            }
        }

        public Guid? ParentTagId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }
    }
}
