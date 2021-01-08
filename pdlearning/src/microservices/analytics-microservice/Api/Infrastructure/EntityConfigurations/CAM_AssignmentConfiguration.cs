using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_AssignmentConfiguration : BaseEntityTypeConfiguration<CAM_Assignment>
    {
        public override void Configure(EntityTypeBuilder<CAM_Assignment> builder)
        {
            builder.ToTable("sam_Departments", "staging");

            builder.HasKey(e => e.AssignmentId);

            builder.ToTable("cam_Assignment", "staging");

            builder.Property(e => e.AssignmentId).ValueGeneratedNever();

            builder.Property(e => e.ChangedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Title).HasMaxLength(100);

            builder.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.HasOne(d => d.ClassRun)
                .WithMany(p => p.CamAssignment)
                .HasForeignKey(d => d.ClassRunId);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamAssignment)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
