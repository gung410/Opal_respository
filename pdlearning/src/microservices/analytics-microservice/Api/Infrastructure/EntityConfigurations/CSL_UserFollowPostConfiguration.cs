using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_UserFollowPostConfiguration : BaseEntityTypeConfiguration<CSL_UserFollowPost>
    {
        public override void Configure(EntityTypeBuilder<CSL_UserFollowPost> builder)
        {
            builder.ToTable("csl_UserFollowPost", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Post)
                .WithMany(p => p.CslUserFollowPost)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_UserF__PostI__3D09E56E");
        }
    }
}
