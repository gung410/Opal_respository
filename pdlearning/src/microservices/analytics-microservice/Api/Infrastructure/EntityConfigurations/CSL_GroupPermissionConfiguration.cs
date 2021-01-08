using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_GroupPermissionConfiguration : BaseEntityTypeConfiguration<CSL_GroupPermission>
    {
        public override void Configure(EntityTypeBuilder<CSL_GroupPermission> builder)
        {
            builder.HasKey(e => new { e.PermissionId, e.GroupId, e.ModuleId });

            builder.ToTable("csl_GroupPermission", "staging");

            builder.Property(e => e.PermissionId)
                .HasMaxLength(150)
                .IsUnicode(false);

            builder.Property(e => e.ModuleId)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.Class)
                .HasMaxLength(255)
                .IsUnicode(false);

            builder.HasOne(d => d.Group)
                .WithMany(p => p.CslGroupPermission)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
