using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class LectureConfiguration : BaseConfiguration<Lecture>
    {
        public override void Configure(EntityTypeBuilder<Lecture> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.LectureName)
                .IsRequired()
                .HasMaxLength(EntitiesConstants.LectureNameLength);

            builder.Property(e => e.LectureIcon)
                .HasMaxLength(EntitiesConstants.LectureIconLength);

            builder.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.ExternalID)
                .HasMaxLength(EntitiesConstants.ExternalIDLength)
                .IsRequired(false);

            builder.HasIndex(p => new { p.CourseId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.SectionId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
        }
    }
}
