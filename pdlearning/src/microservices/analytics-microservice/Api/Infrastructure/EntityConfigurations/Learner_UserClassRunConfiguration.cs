using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserClassRunConfiguration : BaseEntityTypeConfiguration<Learner_UserClassRun>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserClassRun> builder)
        {
            builder.HasKey(e => e.UserClassRunId);

            builder.ToTable("learner_UserClassRun", "staging");

            builder.Property(e => e.UserClassRunId).ValueGeneratedNever();

            builder.Property(e => e.ClassRunChangeStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.LearningStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('NotStarted')");

            builder.Property(e => e.PostCourseEvaluationFormCompleted)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");

            builder.Property(e => e.RateCommentContent).HasMaxLength(2000);

            builder.Property(e => e.RateCommentTitle).HasMaxLength(100);

            builder.Property(e => e.RegistrationType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('Application')");

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.WithdrawalStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.HasOne(d => d.ClassRun)
                .WithMany(p => p.LearnerUserClassRun)
                .HasForeignKey(d => d.ClassRunId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.LearnerUserClassRun)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Registration)
                .WithMany(p => p.LearnerUserClassRun)
                .HasForeignKey(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.LearnerUserClassRun)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
