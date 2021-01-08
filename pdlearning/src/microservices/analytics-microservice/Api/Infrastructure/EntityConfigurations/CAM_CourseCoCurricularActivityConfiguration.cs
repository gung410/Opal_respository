using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseCoCurricularActivityConfiguration : BaseEntityTypeConfiguration<CAM_CourseCoCurricularActivity>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseCoCurricularActivity> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.CoCurricularActivityId });

            builder.ToTable("cam_Course_CoCurricularActivity", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.CoCurricularActivityId).HasColumnName("CoCurricularActivityID");

            builder.HasOne(d => d.CoCurricularActivity)
                .WithMany(p => p.CamCourseCoCurricularActivity)
                .HasForeignKey(d => d.CoCurricularActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseCoCurricularActivity)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
