using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class MetadataTagConfiguration : BaseConfiguration<MetadataTag>
    {
        public override void Configure(EntityTypeBuilder<MetadataTag> builder)
        {
            base.Configure(builder);
            builder.ToTable("MetadataTags");

            builder.Ignore(p => p.TagId);
            builder.HasIndex(p => p.ParentTagId);
            builder.HasIndex(p => p.GroupCode);
            builder.HasIndex(p => p.CodingScheme);
        }
    }
}
