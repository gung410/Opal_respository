using Microservice.WebinarVideoConverter.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Platform.EntityFrameworkCore.Extensions;

namespace Microservice.WebinarVideoConverter.Infrastructure.EntityConfigurations
{
    public class ConvertingTrackingConfiguration : BaseEntityTypeConfiguration<ConvertingTracking>
    {
        public override void Configure(EntityTypeBuilder<ConvertingTracking> builder)
        {
            builder.HasKey(tracking => tracking.Id);
            builder.HasIndex(tracking => tracking.InternalMeetingId).IsUnique();
            builder.Property(tracking => tracking.Status).ConfigureForEnum();
            builder.Property(tracking => tracking.FailedAtStep).ConfigureForEnum();
        }
    }
}
