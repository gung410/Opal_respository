using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Content.Infrastructure.EntityConfigurations
{
    public class ChapterConfiguration : BaseEntityTypeConfiguration<Chapter>
    {
        public override void Configure(EntityTypeBuilder<Chapter> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(e => e.SourceType)
                .ConfigureForEnum();

            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        }
    }
}
