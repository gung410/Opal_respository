using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class PDPM_ODP_LearningDirectionStatusHistoryConfiguration : BaseEntityTypeConfiguration<PDPM_ODP_LearningDirectionStatusHistory>
    {
        public override void Configure(EntityTypeBuilder<PDPM_ODP_LearningDirectionStatusHistory> builder)
        {
            builder.ToTable("pdpm_ODP_LearningDirection_StatusHistory", "staging");

            builder.Property(e => e.Id)
                .HasColumnName("ODP_LearningDirection_StatusId")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.ODP_LearningDirectionId).HasColumnName("ODP_LearningDirectionId");

            builder.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");
        }
    }
}
