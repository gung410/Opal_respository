using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseEasSubstantiveGradeBandingConfiguration : BaseEntityTypeConfiguration<CAM_CourseEasSubstantiveGradeBanding>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseEasSubstantiveGradeBanding> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.LearningFrameworkId });

            builder.ToTable("cam_Course_EasSubstantiveGradeBanding", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseEasSubstantiveGradeBanding)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.LearningFramework)
                .WithMany(p => p.CamCourseEasSubstantiveGradeBanding)
                .HasForeignKey(d => d.LearningFrameworkId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
