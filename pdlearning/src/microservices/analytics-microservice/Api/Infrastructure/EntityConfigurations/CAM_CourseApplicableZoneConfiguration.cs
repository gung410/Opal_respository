using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseApplicableZoneConfiguration : BaseEntityTypeConfiguration<CAM_CourseApplicableZone>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseApplicableZone> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.DepartmentId });

            builder.ToTable("cam_Course_ApplicableZone", "staging");

            builder.Property(e => e.DepartmentId)
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseApplicableZone)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.CamCourseApplicableZone)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
