using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_LectureContentConfiguration : BaseEntityTypeConfiguration<CAM_LectureContent>
    {
        public override void Configure(EntityTypeBuilder<CAM_LectureContent> builder)
        {
            builder.HasKey(e => e.LectureContentId);

            builder.ToTable("cam_LectureContent", "staging");

            builder.Property(e => e.LectureContentId).ValueGeneratedNever();

            builder.Property(e => e.ChangedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.ExternalId)
                .HasColumnName("ExternalID")
                .HasMaxLength(512);

            builder.Property(e => e.MimeType).HasMaxLength(450);

            builder.Property(e => e.QuizConfigByPassPassingRate).HasColumnName("QuizConfig_ByPassPassingRate");

            builder.Property(e => e.Title).HasMaxLength(256);

            builder.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.HasOne(d => d.DigitalContent)
                .WithMany(p => p.CamLectureContent)
                .HasForeignKey(d => d.DigitalContentId);

            builder.HasOne(d => d.Form)
                .WithMany(p => p.CamLectureContent)
                .HasForeignKey(d => d.FormId);

            builder.HasOne(d => d.Lecture)
                .WithMany(p => p.CamLectureContent)
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
