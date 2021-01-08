using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_ClassRunConfiguration : BaseEntityTypeConfiguration<CAM_ClassRun>
    {
        public override void Configure(EntityTypeBuilder<CAM_ClassRun> builder)
        {
            builder.HasKey(e => e.ClassRunId);

            builder.ToTable("cam_ClassRun", "staging");

            builder.Property(e => e.ClassRunId).ValueGeneratedNever();

            builder.Property(e => e.CancellationStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.ChangedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.ClassRunCode).HasMaxLength(450);

            builder.Property(e => e.ClassTitle).HasMaxLength(2000);

            builder.Property(e => e.ContentStatus)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('Draft')");

            builder.Property(e => e.CourseCriteriaActivated)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.RescheduleStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("(N'Unpublished')");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamClassRun)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
