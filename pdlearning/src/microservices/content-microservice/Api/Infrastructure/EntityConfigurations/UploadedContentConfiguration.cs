using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Content.Infrastructure.EntityConfigurations
{
    public class UploadedContentConfiguration : BaseEntityTypeConfiguration<UploadedContent>
    {
        public override void Configure(EntityTypeBuilder<UploadedContent> builder)
        {
            builder.Property(e => e.FileName).HasMaxLength(255);
            builder.Property(e => e.FileLocation).HasMaxLength(1000);
            builder.Property(e => e.FileExtension).HasMaxLength(10).IsUnicode(false);
            builder.Property(e => e.FileType).HasMaxLength(100).IsUnicode(false);

            builder.HasBaseType<DigitalContent>();
        }
    }
}
