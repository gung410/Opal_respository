using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Content.Infrastructure.EntityConfigurations
{
    public class AttributionElementConfiguration : BaseEntityTypeConfiguration<AttributionElement>
    {
        public override void Configure(EntityTypeBuilder<AttributionElement> builder)
        {
            builder.Property(e => e.Source)
                .IsUnicode()
                .HasMaxLength(500);

            builder.Property(e => e.Author)
                .IsUnicode()
                .HasMaxLength(255);

            builder.Property(e => e.Title)
                .IsUnicode()
                .HasMaxLength(255);

            builder.Property(e => e.LicenseType)
                .ConfigureForEnum();
        }
    }
}
