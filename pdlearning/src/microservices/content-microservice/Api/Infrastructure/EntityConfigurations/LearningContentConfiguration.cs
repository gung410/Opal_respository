using Microservice.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Content.Infrastructure.EntityConfigurations
{
    public class LearningContentConfiguration : BaseEntityTypeConfiguration<LearningContent>
    {
        public override void Configure(EntityTypeBuilder<LearningContent> builder)
        {
            builder.Property(e => e.HtmlContent);

            builder.HasBaseType<DigitalContent>();
        }
    }
}
