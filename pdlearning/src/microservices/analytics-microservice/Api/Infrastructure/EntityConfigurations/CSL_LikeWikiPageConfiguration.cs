using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_LikeWikiPageConfiguration : BaseEntityTypeConfiguration<CSL_LikeWikiPage>
    {
        public override void Configure(EntityTypeBuilder<CSL_LikeWikiPage> builder)
        {
            builder.ToTable("csl_LikeWikiPage", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.WikiPage)
                .WithMany(p => p.CslLikeWikiPage)
                .HasForeignKey(d => d.WikiPageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_LikeW__WikiP__2CD37DA5");
        }
    }
}
