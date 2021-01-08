using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_UserFollowSpaceConfiguration : BaseEntityTypeConfiguration<CSL_UserFollowSpace>
    {
        public override void Configure(EntityTypeBuilder<CSL_UserFollowSpace> builder)
        {
            builder.ToTable("csl_UserFollowSpace", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Space)
                .WithMany(p => p.CslUserFollowSpace)
                .HasForeignKey(d => d.SpaceId)
                .HasConstraintName("FK__csl_UserF__Space__30D918B3");
        }
    }
}
