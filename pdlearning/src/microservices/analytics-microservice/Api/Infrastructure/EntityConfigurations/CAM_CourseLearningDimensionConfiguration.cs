using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseLearningDimensionConfiguration : BaseEntityTypeConfiguration<CAM_CourseLearningDimension>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseLearningDimension> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.LearningDimensionId });

            builder.ToTable("cam_Course_LearningDimension", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.LearningDimensionId).HasColumnName("LearningDimensionID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseLearningDimension)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.LearningDimension)
                .WithMany(p => p.CamCourseLearningDimension)
                .HasForeignKey(d => d.LearningDimensionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
