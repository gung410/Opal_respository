using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseCourseOfStudyConfiguration : BaseEntityTypeConfiguration<CAM_CourseCourseOfStudy>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseCourseOfStudy> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.CourseOfStudyId });

            builder.ToTable("cam_Course_CourseOfStudy", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.CourseOfStudyId).HasColumnName("CourseOfStudyID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseCourseOfStudy)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.CourseOfStudy)
                .WithMany(p => p.CamCourseCourseOfStudy)
                .HasForeignKey(d => d.CourseOfStudyId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
