using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseLearningFrameworkConfiguration : BaseEntityTypeConfiguration<CAM_CourseLearningFramework>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseLearningFramework> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.LearningFrameworkId });

            builder.ToTable("cam_Course_LearningFramework", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.LearningFrameworkId).HasColumnName("LearningFrameworkID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseLearningFramework)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.LearningFramework)
                .WithMany(p => p.CamCourseLearningFramework)
                .HasForeignKey(d => d.LearningFrameworkId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
