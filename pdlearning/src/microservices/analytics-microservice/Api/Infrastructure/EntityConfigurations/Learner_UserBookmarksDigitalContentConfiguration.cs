using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserBookmarksDigitalContentConfiguration : BaseEntityTypeConfiguration<Learner_UserBookmarksDigitalContent>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserBookmarksDigitalContent> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("UserBookmarkDigitalContentId")
                .ValueGeneratedOnAdd();

            builder.ToTable("learner_UserBookmarksDigitalContent", "staging");

            builder.Property(e => e.ChangedDate).HasColumnName("UpdateDate");

            builder.Property(e => e.Comment).HasMaxLength(2000);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);

            builder.Property(e => e.UserHistoryId)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.HasOne(d => d.Department)
                .WithMany(p => p.LearnerUserBookmarksDigitalContent)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
