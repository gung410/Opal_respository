using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class SectionConfiguration : BaseConfiguration<Section>
    {
        public override void Configure(EntityTypeBuilder<Section> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(EntitiesConstants.SectionTitleLength);

            builder.Property(e => e.ExternalID)
              .HasMaxLength(EntitiesConstants.ExternalIDLength)
              .IsRequired(false);

            builder.HasIndex(p => new { p.CourseId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
        }
    }
}
