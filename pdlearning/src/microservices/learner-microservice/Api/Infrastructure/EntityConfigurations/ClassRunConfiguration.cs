using System;
using System.Collections.Generic;
using System.Text.Json;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Infrastructure.EntityConfigurations
{
    public class ClassRunConfiguration : BaseEntityTypeConfiguration<ClassRun>
    {
        public override void Configure(EntityTypeBuilder<ClassRun> builder)
        {
            builder.Property(e => e.ClassTitle)
               .HasMaxLength(2000);

            builder.Property(e => e.FacilitatorIds)
                .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<Guid>>(v, null) : null);

            builder.Property(e => e.CoFacilitatorIds)
                .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<Guid>>(v, null) : null);

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<ClassRunStatus>())
               .HasDefaultValue(ClassRunStatus.Unpublished)
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.CancellationStatus)
               .HasConversion(new EnumToStringConverter<ClassRunCancellationStatus>())
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.RescheduleStatus)
               .HasConversion(new EnumToStringConverter<ClassRunRescheduleStatus>())
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.ContentStatus)
               .HasConversion(new EnumToStringConverter<ContentStatus>())
               .HasDefaultValue(ContentStatus.Draft)
               .HasMaxLength(30)
               .IsUnicode(false);

            builder.HasIndex(p => new { p.CourseId, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedDate });
            builder.HasIndex(p => new { p.ChangedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.CreatedDate });
            builder.HasIndex(p => new { p.Status, p.CreatedDate });
            builder.HasIndex(p => new { p.ContentStatus, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunCode, p.CreatedDate });
            builder.HasIndex(p => p.StartDateTime);
        }
    }
}
