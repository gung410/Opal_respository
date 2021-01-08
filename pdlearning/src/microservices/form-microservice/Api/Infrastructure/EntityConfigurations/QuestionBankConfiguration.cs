using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microservice.Form.Domain.ValueObjects.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Form.Infrastructure.EntityConfigurations
{
    public class QuestionBankConfiguration : BaseEntityConfiguration<Domain.Entities.QuestionBank>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.QuestionBank> builder)
        {
            builder.Property(p => p.IsScoreEnabled).HasDefaultValue(true);
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.Property(p => p.QuestionTitle).HasMaxLength(20000).HasColumnType("NTEXT");
            builder
                .Property(p => p.QuestionCorrectAnswer)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<object>(v, null) : (object)null);
            builder
                .Property(e => e.QuestionType)
                .HasConversion(new EnumToStringConverter<QuestionType>())
                .HasColumnType("varchar(30)");
            builder
                .Property(e => e.QuestionOptions)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<QuestionOption>>(v, null) : null);
            base.Configure(builder);
        }
    }
}
