using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_DevelopmentRoleConfiguration : BaseEntityTypeConfiguration<MT_DevelopmentRole>
    {
        public override void Configure(EntityTypeBuilder<MT_DevelopmentRole> builder)
        {
            builder.HasKey(e => e.DevelopmentRoleId);

            builder.ToTable("mt_DevelopmentRole", "staging");

            builder.Property(e => e.DevelopmentRoleId)
                .HasColumnName("DevelopmentRoleID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.ServiceSchemeId).HasColumnName("ServiceSchemeID");

            builder.HasOne(d => d.ServiceScheme)
                .WithMany(p => p.MtDevelopmentRole)
                .HasForeignKey(d => d.ServiceSchemeId);
        }
    }
}
