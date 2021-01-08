using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_CourseLevelConfiguration : BaseEntityTypeConfiguration<MT_CourseLevel>
    {
        public override void Configure(EntityTypeBuilder<MT_CourseLevel> builder)
        {
            builder.HasKey(e => e.CourseLevelId);

            builder.ToTable("mt_CourseLevel", "staging");

            builder.Property(e => e.CourseLevelId)
                .HasColumnName("CourseLevelID")
                .ValueGeneratedNever();

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);
        }
    }
}
