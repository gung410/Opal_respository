using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_EntityStatusReasonsConfiguration : BaseEntityTypeConfiguration<SAM_EntityStatusReasons>
    {
        public override void Configure(EntityTypeBuilder<SAM_EntityStatusReasons> builder)
        {
            builder.HasKey(e => e.EntityStatusReasonId);

            builder.ToTable("sam_EntityStatusReasons", "staging");

            builder.Property(e => e.EntityStatusReasonId).ValueGeneratedNever();

            builder.Property(e => e.CodeName).HasMaxLength(256);

            builder.Property(e => e.Description).HasMaxLength(512);

            builder.Property(e => e.MasterId).HasMaxLength(64);

            builder.Property(e => e.Name).HasMaxLength(512);

            builder.HasOne(d => d.EntityStatus)
                .WithMany(p => p.SamEntityStatusReasons)
                .HasForeignKey(d => d.EntityStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
