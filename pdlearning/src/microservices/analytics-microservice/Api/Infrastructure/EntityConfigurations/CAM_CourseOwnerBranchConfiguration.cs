using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseOwnerBranchConfiguration : BaseEntityTypeConfiguration<CAM_CourseOwnerBranch>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseOwnerBranch> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.DepartmentId });

            builder.ToTable("cam_Course_OwnerBranch", "staging");

            builder.Property(e => e.DepartmentId)
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseOwnerBranch)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.CamCourseOwnerBranch)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
