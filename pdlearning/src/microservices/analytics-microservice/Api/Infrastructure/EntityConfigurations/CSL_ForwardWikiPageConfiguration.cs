using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_ForwardWikiPageConfiguration : BaseEntityTypeConfiguration<CSL_ForwardWikiPage>
    {
        public override void Configure(EntityTypeBuilder<CSL_ForwardWikiPage> builder)
        {
            builder.ToTable("csl_ForwardWikiPage", "staging");

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.WikiPage)
                .WithMany(p => p.CslForwardWikiPage)
                .HasForeignKey(d => d.WikiPageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Forwa__WikiP__234A136B");
        }
    }
}
