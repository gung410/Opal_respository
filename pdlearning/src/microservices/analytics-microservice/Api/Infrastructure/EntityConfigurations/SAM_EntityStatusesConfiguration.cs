using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_EntityStatusesConfiguration : BaseEntityTypeConfiguration<SAM_EntityStatuses>
    {
        public override void Configure(EntityTypeBuilder<SAM_EntityStatuses> builder)
        {
            builder.HasKey(e => e.EntityStatusId);

            builder.ToTable("sam_EntityStatuses", "staging");

            builder.Property(e => e.EntityStatusId).ValueGeneratedNever();

            builder.Property(e => e.CodeName).HasMaxLength(256);

            builder.Property(e => e.Description).HasMaxLength(512);

            builder.Property(e => e.Name).HasMaxLength(512);
        }
    }
}
