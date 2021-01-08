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
    public class AnnouncementConfiguration : BaseConfiguration<Announcement>
    {
        public override void Configure(EntityTypeBuilder<Announcement> builder)
        {
            base.Configure(builder);
            builder.ToTable("Announcement");

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<AnnouncementStatus>())
               .HasMaxLength(30)
               .IsUnicode(false);

            builder.Property(e => e.Participants)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<Guid>>(v, null) : null);

            // Column Indexes
            builder.HasIndex(p => new { p.Title, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.Status, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ScheduleDate, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.CourseId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.ClassrunId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(p => p.CreatedDate);
        }
    }
}
