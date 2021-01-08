using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseApplicableDivisionConfiguration : BaseEntityTypeConfiguration<CAM_CourseApplicableDivision>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseApplicableDivision> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.DepartmentId });

            builder.ToTable("cam_Course_ApplicableDivision", "staging");

            builder.Property(e => e.DepartmentId)
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseApplicableDivision)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.CamCourseApplicableDivision)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
