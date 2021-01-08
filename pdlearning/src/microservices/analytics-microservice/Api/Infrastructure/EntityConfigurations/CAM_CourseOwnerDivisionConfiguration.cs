using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseOwnerDivisionConfiguration : BaseEntityTypeConfiguration<CAM_CourseOwnerDivision>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseOwnerDivision> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.DepartmentId });

            builder.ToTable("cam_Course_OwnerDivision", "staging");

            builder.Property(e => e.DepartmentId)
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseOwnerDivision)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.CamCourseOwnerDivision)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
