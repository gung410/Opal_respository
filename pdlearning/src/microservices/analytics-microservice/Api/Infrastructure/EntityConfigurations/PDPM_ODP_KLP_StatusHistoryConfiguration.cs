using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class PDPM_ODP_KLP_StatusHistoryConfiguration : BaseEntityTypeConfiguration<PDPM_ODP_KLP_StatusHistory>
    {
        public override void Configure(EntityTypeBuilder<PDPM_ODP_KLP_StatusHistory> builder)
        {
            builder.ToTable("pdpm_ODP_KLP_StatusHistory", "staging");

            builder.Property(e => e.Id)
                .HasColumnName("ODP_KLP_StatusId")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.ODP_KLP_Id).HasColumnName("ODP_KLPId");

            builder.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");
        }
    }
}
