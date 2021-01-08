using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_PdperiodConfiguration : BaseEntityTypeConfiguration<MT_Pdperiod>
    {
        public override void Configure(EntityTypeBuilder<MT_Pdperiod> builder)
        {
            builder.HasKey(e => e.PdperiodId);

            builder.ToTable("mt_PDPeriod", "staging");

            builder.Property(e => e.PdperiodId)
                .HasColumnName("PDPeriodId")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);
        }
    }
}
