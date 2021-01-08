using System;

#pragma warning disable SA1201 // Elements should appear in the correct order
namespace Conexus.Opal.Microservice.Metadata.Entities
{
    public class MetadataTag : IMetadataTag
    {
        public int Id { get; set; }

        public Guid TagId { get; set; }

        public Guid? ParentTagId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }
    }

    public interface IMetadataTag
    {
        public Guid TagId { get; set; }

        public Guid? ParentTagId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }
    }
}
#pragma warning restore SA1201 // Elements should appear in the correct order
