using System;
using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums.CourseCriteria;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class CourseCriteriaConfiguration : BaseConfiguration<CourseCriteria>
    {
        public override void Configure(EntityTypeBuilder<CourseCriteria> builder)
        {
            base.Configure(builder);
            builder.ToTable("CourseCriteria");

            builder.Property(e => e.AccountType)
               .HasConversion(new EnumToStringConverter<CourseCriteriaAccountType>())
               .HasMaxLength(30)
               .IsUnicode(false);

            builder.Property(e => e.CourseCriteriaServiceSchemes)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<CourseCriteriaServiceScheme>>(v, null) : null);

            builder.Property(e => e.PlaceOfWork)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<CourseCriteriaPlaceOfWork>(v, null) : null);

            builder.Property(e => e.PreRequisiteCourses)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<Guid>>(v, null) : null);

            builder.Property(e => e.Tracks)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.DevRoles)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TeachingLevels)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TeachingCourseOfStudy)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.JobFamily)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.CoCurricularActivity)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.SubGradeBanding)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            // Column Indexes
            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => new { p.AccountType, p.IsDeleted });
        }
    }
}
