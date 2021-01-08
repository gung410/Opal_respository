using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_WikiPageRevisionConfiguration : BaseEntityTypeConfiguration<CSL_WikiPageRevision>
    {
        public override void Configure(EntityTypeBuilder<CSL_WikiPageRevision> builder)
        {
            builder.ToTable("csl_WikiPageRevision", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.HasOne(d => d.WikiPage)
                .WithMany(p => p.CslWikiPageRevision)
                .HasForeignKey(d => d.WikiPageId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
