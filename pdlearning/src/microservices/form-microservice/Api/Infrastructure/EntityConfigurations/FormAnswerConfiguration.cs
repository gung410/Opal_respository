using System.Text.Json;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.Form.Infrastructure.EntityConfigurations
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

            builder.Property(e => e.PassingStatus)
                .ConfigureForEnum();

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.FormId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.AssignmentId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CourseId, p.IsDeleted, p.CreatedDate });

            base.Configure(builder);
        }
    }
}
