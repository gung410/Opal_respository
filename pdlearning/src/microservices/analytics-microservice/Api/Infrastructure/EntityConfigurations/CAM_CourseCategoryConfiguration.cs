using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseCategoryConfiguration : BaseEntityTypeConfiguration<CAM_CourseCategory>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseCategory> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.CategoryId });

            builder.ToTable("cam_Course_Category", "staging");

            builder.Property(e => e.CategoryId).HasColumnName("CategoryID");

            builder.HasOne(d => d.Category)
                .WithMany(p => p.CamCourseCategory)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseCategory)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
