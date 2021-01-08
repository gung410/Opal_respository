using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserCourseConfiguration : BaseEntityTypeConfiguration<Learner_UserCourse>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserCourse> builder)
        {
            builder.HasKey(e => e.UserCourseId);

            builder.ToTable("learner_UserCourses", "staging");

            builder.Property(e => e.UserCourseId).ValueGeneratedNever();

            builder.Property(e => e.ActualTimeSpent).HasColumnType("decimal(18, 2)");

            builder.Property(e => e.CourseFee).HasColumnType("decimal(18, 2)");

            builder.Property(e => e.CourseType)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            builder.Property(e => e.DepartmentId)
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);

            builder.Property(e => e.DisplayStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.HasContentChanged)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");

            builder.Property(e => e.PostCourseEvaluationFormCompleted).HasDefaultValueSql("(CONVERT([bit],(0)))");

            builder.Property(e => e.RateCommentContent).HasMaxLength(2000);

            builder.Property(e => e.RateCommentTitle).HasMaxLength(100);

            builder.Property(e => e.RegistrationStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.ReviewStatus).HasMaxLength(1000);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.UserHistoryId).HasColumnName("UserHistoryID");

            builder.Property(e => e.Version)
                .HasMaxLength(5)
                .IsUnicode(false);

            builder.Property(e => e.WithdrawalStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.LearnerUserCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.LearnerUserCourses)
                .HasForeignKey(d => d.DepartmentId);

            builder.HasOne(d => d.Registration)
                .WithMany(p => p.LearnerUserCourses)
                .HasForeignKey(d => d.RegistrationId);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.LearnerUserCourses)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
