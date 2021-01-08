using System;
using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class ClassRunConfiguration : BaseConfiguration<ClassRun>
    {
        public override void Configure(EntityTypeBuilder<ClassRun> builder)
        {
            base.Configure(builder);

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

            builder.HasMany(p => p.ClassRunInternalValues)
                .WithOne(p => p.ClassRun)
                .HasForeignKey(p => p.ClassRunId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.CourseAutomateActivated)
                .HasDefaultValue(false);

            builder.HasIndex(p => p.CourseId);
            builder.HasIndex(p => p.ClassRunCode);

            // Because RDS Devop problem, maximum indexing is 1700 byte
            // builder.HasIndex(p => p.ClassTitle);
            builder.HasIndex(p => new { p.Status, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ContentStatus, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunVenueId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CancellationStatus, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.RescheduleStatus, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CourseId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassRunCode, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CourseCriteriaActivated, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CourseAutomateActivated, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => p.StartDateTime);
            builder.HasIndex(p => p.EndDateTime);
            builder.HasIndex(p => p.CreatedDate);
        }
    }
}
