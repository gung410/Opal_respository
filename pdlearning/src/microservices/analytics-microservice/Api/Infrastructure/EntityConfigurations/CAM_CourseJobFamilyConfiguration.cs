using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseJobFamilyConfiguration : BaseEntityTypeConfiguration<CAM_CourseJobFamily>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseJobFamily> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.JobFamilyId });

            builder.ToTable("cam_Course_JobFamily", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.JobFamilyId).HasColumnName("JobFamilyID");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseJobFamily)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.JobFamily)
                .WithMany(p => p.CamCourseJobFamily)
                .HasForeignKey(d => d.JobFamilyId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
