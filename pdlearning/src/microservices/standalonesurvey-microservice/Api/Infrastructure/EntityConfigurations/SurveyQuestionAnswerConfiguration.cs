using System.Text.Json;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.StandaloneSurvey.Infrastructure.EntityConfigurations
{
    public class SurveyQuestionAnswerConfiguration : BaseEntityConfiguration<SurveyQuestionAnswer>
    {
        public override void Configure(EntityTypeBuilder<SurveyQuestionAnswer> builder)
        {
            builder
                .Property(p => p.AnswerValue)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<object>(v, null) : null);

            builder.HasIndex(p => new { p.CreatedBy, FormAnswerId = p.SurveyAnswerId, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.CreatedDate });
            builder.HasIndex(p => new { FormAnswerId = p.SurveyAnswerId, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
