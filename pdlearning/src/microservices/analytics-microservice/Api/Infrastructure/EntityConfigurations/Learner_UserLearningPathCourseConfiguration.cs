using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserLearningPathCourseConfiguration : BaseEntityTypeConfiguration<Learner_UserLearningPathCourse>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserLearningPathCourse> builder)
        {
            builder.HasKey(e => e.UserLearningPathCoursesId);

            builder.ToTable("learner_UserLearningPathCourses", "staging");

            builder.Property(e => e.UserLearningPathCoursesId).ValueGeneratedNever();

            builder.HasOne(d => d.UserLearningPath)
                .WithMany(p => p.LearnerUserLearningPathCourses)
                .HasForeignKey(d => d.UserLearningPathId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
