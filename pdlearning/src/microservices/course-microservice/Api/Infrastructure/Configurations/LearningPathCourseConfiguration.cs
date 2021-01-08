using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class LearningPathCourseConfiguration : BaseConfiguration<LearningPathCourse>
    {
        public override void Configure(EntityTypeBuilder<LearningPathCourse> builder)
        {
            base.Configure(builder);

            builder.HasIndex(p => new { p.LearningPathId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CourseId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.Order, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
        }
    }
}
