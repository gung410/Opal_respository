using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_LikePostConfiguration : BaseEntityTypeConfiguration<CSL_LikePost>
    {
        public override void Configure(EntityTypeBuilder<CSL_LikePost> builder)
        {
            builder.ToTable("csl_LikePost", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.HasOne(d => d.Post)
                .WithMany(p => p.CslLikePost)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_LikeP__PostI__3A2D78C3");
        }
    }
}
