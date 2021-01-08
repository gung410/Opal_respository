using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseCoFacilitatorConfiguration : BaseEntityTypeConfiguration<CAM_CourseCoFacilitator>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseCoFacilitator> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.UserId });

            builder.ToTable("cam_Course_coFacilitator", "staging");

            builder.Property(e => e.UserId).HasColumnName("userId");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseCoFacilitator)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
