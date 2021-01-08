using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_PdactivityTypeConfiguration : BaseEntityTypeConfiguration<MT_PdactivityType>
    {
        public override void Configure(EntityTypeBuilder<MT_PdactivityType> builder)
        {
            builder.HasKey(e => e.PdactivityTypeId);

            builder.ToTable("mt_PDActivityType", "staging");

            builder.Property(e => e.PdactivityTypeId)
                .HasColumnName("PDActivityTypeID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);
        }
    }
}
