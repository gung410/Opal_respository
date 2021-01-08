using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class CourseReviewConfiguration : BaseEntityTypeConfiguration<CourseReview>
    {
        public override void Configure(EntityTypeBuilder<CourseReview> builder)
        {
            builder.Property(e => e.Version)
                .HasColumnType("varchar(100)");

            builder.Property(e => e.UserFullName)
                .HasMaxLength(CourseReview.MaxUserFullNameLength);

            builder.Property(e => e.CommentTitle)
                .HasMaxLength(CourseReview.MaxCommentTitleLength);

            builder.Property(e => e.CommentContent)
                .HasMaxLength(CourseReview.MaxCommentContentLength);

            builder.Property(e => e.ItemName)
                .HasMaxLength(CourseReview.MaxItemNameLength);

            // Restrict user to have multiple reviews for one course
            builder.HasIndex(p => new { p.UserId, p.CourseId }).IsUnique();
        }
    }
}
