using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseFacilitatorConfiguration : BaseEntityTypeConfiguration<CAM_CourseFacilitator>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseFacilitator> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.UserId });

            builder.ToTable("cam_Course_Facilitator", "staging");

            builder.Property(e => e.UserId).HasColumnName("userId");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseFacilitator)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
