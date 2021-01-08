using System;
using System.Collections.Generic;
using System.Text.Json;
using Microservice.Form.Domain;
using Microservice.Form.Domain.ValueObjects.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Form.Infrastructure.EntityConfigurations
{
    internal static class QuestionConfigurationHelper
    {
        public static void ConfigOwnQuestionEntity<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : BaseEntity, IOwnQuestionEntity
        {
            builder.Property(p => p.Question_Title).HasMaxLength(20000).HasColumnType("NTEXT");
            builder
                .Property(p => p.Question_CorrectAnswer)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<object>(v, null) : (object)null);
            builder
                .Property(e => e.Question_Type)
                .HasConversion(new EnumToStringConverter<QuestionType>())
                .HasColumnType("varchar(30)");
            builder
                .Property(e => e.Question_Options)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<QuestionOption>>(v, null) : null);
        }

        public static Action<OwnedNavigationBuilder<TEntity, Question>> CreateQuestionBuildAction<TEntity>(
            Action<OwnedNavigationBuilder<TEntity, Question>> additionalAction = null) where TEntity : BaseEntity
        {
            return modelBuilder =>
            {
                modelBuilder.Property(p => p.Title).HasMaxLength(3000);
                modelBuilder
                    .Property(p => p.CorrectAnswer)
                    .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<object>(v, null) : null);
                modelBuilder
                    .Property(e => e.Type)
                    .HasConversion(new EnumToStringConverter<QuestionType>())
                    .HasColumnType("varchar(30)");
                modelBuilder
                    .Property(e => e.Options)
                    .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<QuestionOption>>(v, null) : null);

                additionalAction?.Invoke(modelBuilder);
            };
        }
    }
}
