using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Content.Infrastructure.EntityConfigurations
{
    public class VideoCommentConfiguration : BaseEntityTypeConfiguration<VideoComment>
    {
        public override void Configure(EntityTypeBuilder<VideoComment> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(e => e.SourceType)
                .ConfigureForEnum();

            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        }
    }
}
