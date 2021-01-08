using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class PDPM_IDP_PDO_StatusHistoryConfiguration : BaseEntityTypeConfiguration<PDPM_IDP_PDO_StatusHistory>
    {
        public override void Configure(EntityTypeBuilder<PDPM_IDP_PDO_StatusHistory> builder)
        {
            builder.ToTable("pdpm_IDP_PDO_StatusHistory", "staging");

            builder.Property(e => e.Id)
                .HasColumnName("IDP_PDO_StatusID")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.IDP_PDO_Id).HasColumnName("IDP_PDOId");

            builder.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");
        }
    }
}
