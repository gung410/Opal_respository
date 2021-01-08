using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseTeachingSubjectConfiguration : BaseEntityTypeConfiguration<CAM_CourseTeachingSubject>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseTeachingSubject> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.TeachingSubjectId });

            builder.ToTable("cam_Course_TeachingSubject", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.TeachingSubjectId).HasColumnName("TeachingSubjectID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseTeachingSubject)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.TeachingSubject)
                .WithMany(p => p.CamCourseTeachingSubject)
                .HasForeignKey(d => d.TeachingSubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
