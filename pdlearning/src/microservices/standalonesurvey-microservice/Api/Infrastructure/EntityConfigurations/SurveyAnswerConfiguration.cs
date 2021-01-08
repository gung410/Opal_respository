using System.Text.Json;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.StandaloneSurvey.Infrastructure.EntityConfigurations
{
    public class SurveyAnswerConfiguration : BaseEntityConfiguration<SurveyAnswer>
    {
        public override void Configure(EntityTypeBuilder<SurveyAnswer> builder)
        {
            builder
                .Property(p => p.SurveyAnswerFormMetaData)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<SurveyAnswerFormMetaData>(v, null) : null);
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.FormId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.OwnerId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ResourceId, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
