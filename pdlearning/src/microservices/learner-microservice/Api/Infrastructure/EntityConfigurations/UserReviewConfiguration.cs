using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class UserReviewConfiguration : BaseEntityTypeConfiguration<UserReview>
    {
        public override void Configure(EntityTypeBuilder<UserReview> builder)
        {
            builder.Property(e => e.Version)
                .HasColumnType("varchar(100)");

            builder.Property(e => e.ItemType)
                .HasConversion(new EnumToStringConverter<ItemType>())
                .HasColumnType("varchar(30)");

            builder.Property(e => e.UserFullName)
                .HasMaxLength(UserReview.MaxUserFullNameLength);

            builder.Property(e => e.CommentTitle)
                .HasMaxLength(UserReview.MaxCommentTitleLength);

            builder.Property(e => e.CommentContent)
                .HasMaxLength(UserReview.MaxCommentContentLength);

            builder.Property(e => e.ItemName)
                .HasMaxLength(UserReview.MaxItemNameLength);

            builder.HasIndex(p => new { p.UserId, p.ItemId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ItemId, p.UserId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.UserId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ItemId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ItemId, p.Rate, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ItemType, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ParentCommentId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.Rate, p.IsDeleted, p.CreatedDate });
        }
    }
}
