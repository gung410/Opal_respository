using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class PDPM_IDP_LNA_StatusHistoryConfiguration : BaseEntityTypeConfiguration<PDPM_IDP_LNA_StatusHistory>
    {
        public override void Configure(EntityTypeBuilder<PDPM_IDP_LNA_StatusHistory> builder)
        {
            builder.ToTable("pdpm_IDP_LNA_StatusHistory", "staging");

            builder.Property(e => e.Id)
                .HasColumnName("IDP_LNA_StatusID")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.IDP_LNA_Id).HasColumnName("IDP_LNAId");

            builder.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");
        }
    }
}
