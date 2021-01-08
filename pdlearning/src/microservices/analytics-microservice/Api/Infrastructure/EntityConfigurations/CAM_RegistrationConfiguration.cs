using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_RegistrationConfiguration : BaseEntityTypeConfiguration<CAM_Registration>
    {
        public override void Configure(EntityTypeBuilder<CAM_Registration> builder)
        {
            builder.HasKey(e => e.RegistrationId);

            builder.ToTable("cam_Registration", "staging");

            builder.Property(e => e.RegistrationId).ValueGeneratedNever();

            builder.Property(e => e.AdministratedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.ChangedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.ClassRunChangeStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.LearningStatus)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('NotStarted')");

            builder.Property(e => e.PostCourseEvaluationFormCompleted)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");

            builder.Property(e => e.RegistrationType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("(N'Application')");

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("(N'PendingConfirmation')");

            builder.Property(e => e.WithdrawalStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.HasOne(d => d.ClassRun)
                .WithMany(p => p.CamRegistration)
                .HasForeignKey(d => d.ClassRunId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamRegistration)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.CamRegistration)
                .HasForeignKey(d => d.UserHistoryId);
        }
    }
}
