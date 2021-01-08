using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class LearnerLearningPathConfiguration : BaseEntityTypeConfiguration<LearnerLearningPath>
    {
        public override void Configure(EntityTypeBuilder<LearnerLearningPath> builder)
        {
            builder.Property(e => e.Title)
                .HasMaxLength(1000);

            builder.Property(e => e.ThumbnailUrl)
                .HasMaxLength(300);
        }
    }
}
