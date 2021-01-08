using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class PDPM_ODP_LearningPlanStatusHistoryConfiguration : BaseEntityTypeConfiguration<PDPM_ODP_LearningPlanStatusHistory>
    {
        public override void Configure(EntityTypeBuilder<PDPM_ODP_LearningPlanStatusHistory> builder)
        {
            builder.ToTable("pdpm_ODP_LearningPlan_StatusHistory", "staging");

            builder.Property(e => e.Id)
                .HasColumnName("ODP_LearningPlan_StatusID")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.ODP_LearningPlanId).HasColumnName("ODP_LearningPlanID");

            builder.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");
        }
    }
}
