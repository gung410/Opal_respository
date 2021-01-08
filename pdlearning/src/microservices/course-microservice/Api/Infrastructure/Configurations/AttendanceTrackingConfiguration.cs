using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class AttendanceTrackingConfiguration : BaseConfiguration<AttendanceTracking>
    {
        public override void Configure(EntityTypeBuilder<AttendanceTracking> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<AttendanceTrackingStatus>())
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.Attachment)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.HasIndex(p => new { p.SessionId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.RegistrationId, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.Userid, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.Status, p.IsDeleted, p.CreatedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.CreatedDate });
        }
    }
}
