using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseLearningSubAreaConfiguration : BaseEntityTypeConfiguration<CAM_CourseLearningSubArea>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseLearningSubArea> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.LearningSubAreaId });

            builder.ToTable("cam_Course_LearningSubArea", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.LearningSubAreaId).HasColumnName("LearningSubAreaID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseLearningSubArea)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.LearningSubArea)
                .WithMany(p => p.CamCourseLearningSubArea)
                .HasForeignKey(d => d.LearningSubAreaId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
