using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class QuizAssignmentFormQuestionConfiguration : BaseConfiguration<QuizAssignmentFormQuestion>
    {
        public override void Configure(EntityTypeBuilder<QuizAssignmentFormQuestion> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.Question_Title).HasMaxLength(20000).HasColumnType("NTEXT");
            builder
                .Property(p => p.Question_CorrectAnswer)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<object>(v, null) : null);
            builder
                .Property(e => e.Question_Type)
                .HasConversion(new EnumToStringConverter<QuestionType>())
                .HasColumnType("varchar(30)");
            builder
                .Property(e => e.Question_Options)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<QuestionOption>>(v, null) : null);
            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => p.QuizAssignmentFormId);
            builder.HasIndex(p => new { p.IsDeleted, p.Priority });
            builder.HasIndex(p => new { p.Question_Type, p.IsDeleted });
        }
    }
}
