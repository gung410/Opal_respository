using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CCPM_DigitalContentTrackingConfiguration : BaseEntityTypeConfiguration<CCPM_DigitalContentTracking>
    {
        public override void Configure(EntityTypeBuilder<CCPM_DigitalContentTracking> builder)
        {
            builder.ToTable("ccpm_DigitalContent_Tracking", "staging");

            builder.Property(e => e.Id)
                .HasColumnName("DigitalContent_TrackingId")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.ChangedDate)
                .HasColumnName("UpdatedDate");

            builder.Property(e => e.Action).HasMaxLength(64);

            builder.Property(e => e.DepartmentId).HasMaxLength(64);
        }
    }
}
