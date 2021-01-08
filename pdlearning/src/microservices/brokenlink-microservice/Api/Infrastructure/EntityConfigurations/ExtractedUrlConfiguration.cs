using Microservice.BrokenLink.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.BrokenLink.Infrastructure.EntityConfigurations
{
    public class ExtractedUrlConfiguration : BaseEntityTypeConfiguration<ExtractedUrl>
    {
        public override void Configure(EntityTypeBuilder<ExtractedUrl> builder)
        {
            builder.Property(e => e.Status)
                .ConfigureForEnum();

            builder.Property(e => e.Module)
                .ConfigureForEnum();

            builder.Property(e => e.ContentType)
                .ConfigureForEnum();

            builder.Property(e => e.Url).HasMaxLength(5000);

            builder.Property(e => e.ObjectTitle).HasMaxLength(5000);

            builder.Property(e => e.ObjectOwnerName).HasMaxLength(500);

            builder.Property(e => e.ObjectDetailUrl).HasMaxLength(2000);
        }
    }
}
