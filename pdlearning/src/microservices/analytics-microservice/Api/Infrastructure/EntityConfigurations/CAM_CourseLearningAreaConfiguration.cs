using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseLearningAreaConfiguration : BaseEntityTypeConfiguration<CAM_CourseLearningArea>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseLearningArea> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.LearningAreaId });

            builder.ToTable("cam_Course_LearningArea", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.LearningAreaId).HasColumnName("LearningAreaID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseLearningArea)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.LearningArea)
                .WithMany(p => p.CamCourseLearningArea)
                .HasForeignKey(d => d.LearningAreaId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
