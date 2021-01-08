using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class LearningPathConfiguration : BaseConfiguration<LearningPath>
    {
        public override void Configure(EntityTypeBuilder<LearningPath> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Title)
               .IsRequired()
               .HasMaxLength(100);

            builder.Property(e => e.ThumbnailUrl)
                  .HasMaxLength(300)
                  .IsUnicode(false);

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<LearningPathStatus>())
               .HasDefaultValue(LearningPathStatus.Unpublished)
               .HasMaxLength(30)
               .IsUnicode(false);

            // Metadata
            builder.Property(e => e.CourseLevelIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.PDAreaThemeIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.ServiceSchemeIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.SubjectAreaIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.LearningFrameworkIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.LearningDimensionIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.LearningAreaIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.LearningSubAreaIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.MetadataKeys)
                .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.HasIndex(p => new { p.Status, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
        }
    }
}
