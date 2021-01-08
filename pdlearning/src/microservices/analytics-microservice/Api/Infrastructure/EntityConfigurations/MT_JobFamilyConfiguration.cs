using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_JobFamilyConfiguration : BaseEntityTypeConfiguration<MT_JobFamily>
    {
        public override void Configure(EntityTypeBuilder<MT_JobFamily> builder)
        {
            builder.HasKey(e => e.JobFamilyId);

            builder.ToTable("mt_JobFamily", "staging");

            builder.Property(e => e.JobFamilyId)
                .HasColumnName("JobFamilyID")
                .ValueGeneratedNever();

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);
        }
    }
}
