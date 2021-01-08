using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_CommentWikiPageConfiguration : BaseEntityTypeConfiguration<CSL_CommentWikiPage>
    {
        public override void Configure(EntityTypeBuilder<CSL_CommentWikiPage> builder)
        {
            builder.ToTable("csl_CommentWikiPage", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.WikiPage)
                .WithMany(p => p.CslCommentWikiPage)
                .HasForeignKey(d => d.WikiPageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Comme__WikiP__18CC84F8");
        }
    }
}
