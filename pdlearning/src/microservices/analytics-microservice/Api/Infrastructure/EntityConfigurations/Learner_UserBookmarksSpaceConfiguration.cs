using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserBookmarksSpaceConfiguration : BaseEntityTypeConfiguration<Learner_UserBookmarksSpace>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserBookmarksSpace> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("UserBookmarkSpaceId").ValueGeneratedOnAdd();

            builder.Property(e => e.ChangedDate).HasColumnName("UpdateDate");

            builder.ToTable("learner_UserBookmarksSpace", "staging");

            builder.Property(e => e.Comment).HasMaxLength(2000);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);
        }
    }
}
