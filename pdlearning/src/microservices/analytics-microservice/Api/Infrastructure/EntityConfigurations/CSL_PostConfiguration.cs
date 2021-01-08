using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_PostConfiguration : BaseEntityTypeConfiguration<CSL_Post>
    {
        public override void Configure(EntityTypeBuilder<CSL_Post> builder)
        {
            builder.ToTable("csl_Post", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false);
        }
    }
}
