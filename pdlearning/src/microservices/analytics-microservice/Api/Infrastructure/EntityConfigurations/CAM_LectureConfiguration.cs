using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_LectureConfiguration : BaseEntityTypeConfiguration<CAM_Lecture>
    {
        public override void Configure(EntityTypeBuilder<CAM_Lecture> builder)
        {
            builder.HasKey(e => e.LectureId);

            builder.ToTable("cam_Lecture", "staging");

            builder.Property(e => e.LectureId).ValueGeneratedNever();

            builder.Property(e => e.ChangedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.ExternalId)
                .HasColumnName("ExternalID")
                .HasMaxLength(512);

            builder.Property(e => e.LectureName)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.HasOne(d => d.ClassRun)
                .WithMany(p => p.CamLecture)
                .HasForeignKey(d => d.ClassRunId);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamLecture)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Section)
                .WithMany(p => p.CamLecture)
                .HasForeignKey(d => d.SectionId);
        }
    }
}
