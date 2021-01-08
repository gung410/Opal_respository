using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserLearningPathConfiguration : BaseEntityTypeConfiguration<Learner_UserLearningPath>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserLearningPath> builder)
        {
            builder.HasKey(e => e.UserLearningPathId);

            builder.ToTable("learner_UserLearningPaths", "staging");

            builder.Property(e => e.UserLearningPathId).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.IsPublic)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");

            builder.Property(e => e.ThumbnailUrl).HasMaxLength(300);

            builder.Property(e => e.Title).HasMaxLength(1000);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.LearnerUserLearningPaths)
                .HasForeignKey(d => d.UserHistoryId);
        }
    }
}
