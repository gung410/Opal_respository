using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserBookmarksLearnerLearningPathConfiguration : BaseEntityTypeConfiguration<Learner_UserBookmarksLearnerLearningPath>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserBookmarksLearnerLearningPath> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("UserBookmarkLearnerLearningPathId").ValueGeneratedOnAdd();

            builder.Property(e => e.ChangedDate).HasColumnName("UpdateDate");

            builder.ToTable("learner_UserBookmarksLearnerLearningPath", "staging");

            builder.Property(e => e.Comment).HasMaxLength(2000);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);
        }
    }
}
