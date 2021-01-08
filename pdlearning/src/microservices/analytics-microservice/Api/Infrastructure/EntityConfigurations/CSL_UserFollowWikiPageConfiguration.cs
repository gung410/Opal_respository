using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_UserFollowWikiPageConfiguration : BaseEntityTypeConfiguration<CSL_UserFollowWikiPage>
    {
        public override void Configure(EntityTypeBuilder<CSL_UserFollowWikiPage> builder)
        {
            builder.ToTable("csl_UserFollowWikiPage", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.HasOne(d => d.WikiPage)
                .WithMany(p => p.CslUserFollowWikiPage)
                .HasForeignKey(d => d.WikiPageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_UserF__WikiP__1313ABA2");
        }
    }
}
