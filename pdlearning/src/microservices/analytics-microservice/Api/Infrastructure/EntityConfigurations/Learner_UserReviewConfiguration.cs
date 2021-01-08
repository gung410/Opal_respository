using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserReviewConfiguration : BaseEntityTypeConfiguration<Learner_UserReview>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserReview> builder)
        {
            builder.HasKey(e => e.UserReviewsId);

            builder.ToTable("learner_UserReviews", "staging");

            builder.Property(e => e.UserReviewsId).ValueGeneratedNever();

            builder.Property(e => e.CommentContent).HasMaxLength(2000);

            builder.Property(e => e.CommentTitle).HasMaxLength(100);

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.ItemName).HasMaxLength(500);

            builder.Property(e => e.ItemType)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);

            builder.Property(e => e.UserFullName).HasMaxLength(500);

            builder.Property(e => e.Version)
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.HasOne(d => d.ClassRun)
                .WithMany(p => p.LearnerUserReviews)
                .HasForeignKey(d => d.ClassRunId);

            builder.HasOne(d => d.Course)
                .WithMany(p => p.LearnerUserReviews)
                .HasForeignKey(d => d.CourseId);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.LearnerUserReviews)
                .HasForeignKey(d => d.DepartmentId);

            builder.HasOne(d => d.DigitalContent)
                .WithMany(p => p.LearnerUserReviews)
                .HasForeignKey(d => d.DigitalContentId);
        }
    }
}
