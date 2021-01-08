using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class ECertificateTemplateConfiguration : BaseConfiguration<ECertificateTemplate>
    {
        public override void Configure(EntityTypeBuilder<ECertificateTemplate> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.FullTextSearchKey)
                   .IsRequired(true)
                   .IsUnicode(false)
                   .HasColumnType("nvarchar(200)")
                   .HasDefaultValue(string.Empty);

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<ECertificateTemplateStatus>())
               .HasDefaultValue(ECertificateTemplateStatus.Draft)
               .HasMaxLength(30)
               .IsUnicode(false);

            builder.Property(e => e.Params)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, new JsonSerializerOptions
                    {
                        Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                    }) : null,
                    v => v != null ? JsonSerializer.Deserialize<List<ECertificateTemplateParam>>(v, new JsonSerializerOptions
                    {
                        Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                    }) : null);

            builder.Property(e => e.IsSystem)
                .HasDefaultValue(false);

            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => p.CreatedDate);
            builder.HasIndex(p => new { p.Status, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.ECertificateLayoutId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.IsSystem, p.IsDeleted, p.FullTextSearchKey });

            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.IsDeleted, p.ChangedDate });

            // Technical Columns Indexes
            builder.HasIndex(p => p.FullTextSearchKey).IsUnique();
        }
    }
}
