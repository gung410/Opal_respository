using System.Collections.Generic;
using System.Text.Json;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.StandaloneSurvey.Infrastructure.EntityConfigurations
{
    public class SurveyQuestionConfiguration : BaseEntityConfiguration<SurveyQuestion>
    {
        public override void Configure(EntityTypeBuilder<SurveyQuestion> builder)
        {
            builder.Property(p => p.Priority).HasDefaultValue(0);
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);
            builder.Property(p => p.Title).HasMaxLength(20000).HasColumnType("NTEXT");

            builder
                .Property(p => p.CorrectAnswer)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<object>(v, null) : (object)null);
            builder
                .Property(e => e.QuestionType)
                .HasConversion(new EnumToStringConverter<QuestionType>())
                .HasColumnType("varchar(30)");
            builder
                .Property(e => e.Options)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<QuestionOption>>(v, null) : null);

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { FormId = p.SurveyId, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
