using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseServiceSchemeConfiguration : BaseEntityTypeConfiguration<CAM_CourseServiceScheme>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseServiceScheme> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.ServiceSchemeId });

            builder.ToTable("cam_Course_ServiceScheme", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.ServiceSchemeId).HasColumnName("ServiceSchemeID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseServiceScheme)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.ServiceScheme)
                .WithMany(p => p.CamCourseServiceScheme)
                .HasForeignKey(d => d.ServiceSchemeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
