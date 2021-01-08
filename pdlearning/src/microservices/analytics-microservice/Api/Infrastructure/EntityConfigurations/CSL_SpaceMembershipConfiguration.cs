using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_SpaceMembershipConfiguration : BaseEntityTypeConfiguration<CSL_SpaceMembership>
    {
        public override void Configure(EntityTypeBuilder<CSL_SpaceMembership> builder)
        {
            builder.ToTable("csl_SpaceMembership", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.AssignedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.LastVisitDate).HasColumnType("datetime");

            builder.Property(e => e.MembershipType)
                .HasMaxLength(255)
                .IsUnicode(false);

            builder.HasOne(d => d.Space)
                .WithMany(p => p.CslSpaceMembership)
                .HasForeignKey(d => d.SpaceId)
                .HasConstraintName("FK__csl_Space__Space__57F2E5D4");

            builder.HasOne(d => d.User)
                .WithMany(p => p.CslSpaceMembership)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Space__UserI__58E70A0D");
        }
    }
}
