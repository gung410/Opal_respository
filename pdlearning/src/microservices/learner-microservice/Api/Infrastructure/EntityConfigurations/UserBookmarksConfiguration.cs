using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class UserBookmarksConfiguration : BaseEntityTypeConfiguration<UserBookmark>
    {
        public override void Configure(EntityTypeBuilder<UserBookmark> builder)
        {
            builder.Property(e => e.ItemName)
                .HasMaxLength(UserBookmark.MaxItemNameLength);

            builder.Property(e => e.Comment)
                .HasMaxLength(UserBookmark.MaxCommentLength);

            builder.Property(e => e.ItemType)
                .HasConversion(new EnumToStringConverter<BookmarkType>())
                .HasColumnType("varchar(30)");

            // There is a issue where a user can insert multiple bookmarks for one course.
            // It happens because we don't restrict user to have just only one bookmark for the course.
            // It really easy to get into this issue for example: user clicks on bookmark button very fast that lead to multiple requests at the same time.
            // This constraint is to ensure there is no duplicated rows by UserId + ItemId.
            builder.HasIndex(p => new { p.UserId, p.ItemId }).IsUnique();

            builder.HasIndex(p => new { p.UserId, p.ItemType, p.CreatedDate });
        }
    }
}
