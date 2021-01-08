using Microservice.Content.Versioning.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Content.Infrastructure.EntityConfigurations
{
    public class VersionTrackingConfiguration : BaseEntityTypeConfiguration<VersionTracking>
    {
        public override void Configure(EntityTypeBuilder<VersionTracking> builder)
        {
            builder.HasIndex(p => new { p.OriginalObjectId, p.MajorVersion, p.CreatedDate });
            builder.HasIndex(p => new { p.OriginalObjectId, p.CreatedDate });
        }
    }
}
