using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class AssignmentConfiguration : BaseConfiguration<Assignment>
    {
        public override void Configure(EntityTypeBuilder<Assignment> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Title)
                .HasMaxLength(EntitiesConstants.AssignmentTitleLength);

            builder.Property(e => e.Type)
                .HasConversion(new EnumToStringConverter<AssignmentType>())
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.OwnsOne(p => p.AssessmentConfig)
                .Property(p => p.ScoreMode)
                .HasConversion(new EnumToStringConverter<ScoreMode>());

            builder.HasIndex(p => new { p.CourseId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.Type, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
        }
    }
}
