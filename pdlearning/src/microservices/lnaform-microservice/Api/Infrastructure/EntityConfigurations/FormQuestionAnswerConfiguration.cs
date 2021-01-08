using System.Text.Json;
using Microservice.LnaForm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.LnaForm.Infrastructure.EntityConfigurations
{
    public class FormQuestionAnswerConfiguration : BaseEntityConfiguration<FormQuestionAnswer>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.FormQuestionAnswer> builder)
        {
            builder
                .Property(p => p.AnswerValue)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<object>(v, null) : null);

            builder.HasIndex(p => new { p.CreatedBy, p.FormAnswerId, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.CreatedDate });
            builder.HasIndex(p => new { p.FormAnswerId, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
