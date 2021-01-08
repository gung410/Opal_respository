using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class LearnerLearningPathCourseConfiguration : BaseEntityTypeConfiguration<LearnerLearningPathCourse>
    {
        public override void Configure(EntityTypeBuilder<LearnerLearningPathCourse> builder)
        {
        }
    }
}
