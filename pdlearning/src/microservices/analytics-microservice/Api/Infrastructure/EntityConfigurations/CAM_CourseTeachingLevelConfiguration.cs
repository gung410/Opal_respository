using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseTeachingLevelConfiguration : BaseEntityTypeConfiguration<CAM_CourseTeachingLevel>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseTeachingLevel> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.TeachingLevelId });

            builder.ToTable("cam_Course_TeachingLevel", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.TeachingLevelId).HasColumnName("TeachingLevelID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseTeachingLevel)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.TeachingLevel)
                .WithMany(p => p.CamCourseTeachingLevel)
                .HasForeignKey(d => d.TeachingLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
