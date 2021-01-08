using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseDevelopmentRoleConfiguration : BaseEntityTypeConfiguration<CAM_CourseDevelopmentRole>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseDevelopmentRole> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.DevelopmentRoleId });

            builder.ToTable("cam_Course_DevelopmentRole", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.DevelopmentRoleId).HasColumnName("DevelopmentRoleID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseDevelopmentRole)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.DevelopmentRole)
                .WithMany(p => p.CamCourseDevelopmentRole)
                .HasForeignKey(d => d.DevelopmentRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
