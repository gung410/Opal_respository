using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CoursePreRequisiteConfiguration : BaseEntityTypeConfiguration<CAM_CoursePreRequisite>
    {
        public override void Configure(EntityTypeBuilder<CAM_CoursePreRequisite> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.PreRequisiteCourseId });

            builder.ToTable("cam_Course_PreRequisite", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.PreRequisiteCourseId).HasColumnName("PreRequisiteCourseID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCoursePreRequisiteCourse)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.PreRequisiteCourse)
                .WithMany(p => p.CamCoursePreRequisitePreRequisiteCourse)
                .HasForeignKey(d => d.PreRequisiteCourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
