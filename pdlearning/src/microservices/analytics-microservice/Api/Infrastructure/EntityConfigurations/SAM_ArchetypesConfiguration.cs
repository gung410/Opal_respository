using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_ArchetypesConfiguration : BaseEntityTypeConfiguration<SAM_Archetypes>
    {
        public override void Configure(EntityTypeBuilder<SAM_Archetypes> builder)
        {
            builder.HasKey(e => e.ArcheTypeId);

            builder.ToTable("sam_Archetypes", "staging");

            builder.Property(e => e.ArcheTypeId).HasMaxLength(64);

            builder.Property(e => e.CodeName).HasMaxLength(256);

            builder.Property(e => e.MasterId).HasMaxLength(64);

            builder.Property(e => e.TableTypeId).HasMaxLength(256);
        }
    }
}
