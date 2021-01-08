using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_GroupUserConfiguration : BaseEntityTypeConfiguration<CSL_GroupUser>
    {
        public override void Configure(EntityTypeBuilder<CSL_GroupUser> builder)
        {
            builder.ToTable("csl_GroupUser", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.AssignedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Group)
                .WithMany(p => p.CslGroupUser)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Group__Group__1DD065E0");

            builder.HasOne(d => d.User)
                .WithMany(p => p.CslGroupUser)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Group__UserI__1CDC41A7");
        }
    }
}
