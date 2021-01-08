using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_SpaceConfiguration : BaseEntityTypeConfiguration<CSL_Space>
    {
        public override void Configure(EntityTypeBuilder<CSL_Space> builder)
        {
            builder.ToTable("csl_Space", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.CoursesId)
                .HasMaxLength(64)
                .IsUnicode(false);

            builder.Property(e => e.ClassRunId)
                .HasMaxLength(64)
                .IsUnicode(false);

            builder.Property(e => e.Name).HasMaxLength(500);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false);
        }
    }
}
