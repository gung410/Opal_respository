using System.Text.Json;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.LnaForm.Infrastructure.EntityConfigurations
{
    public class FormAnswerConfiguration : BaseEntityConfiguration<FormAnswer>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.FormAnswer> builder)
        {
            builder
                .Property(p => p.FormMetaData)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<FormAnswerFormMetaData>(v, null) : null);
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.FormId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.OwnerId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ResourceId, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
