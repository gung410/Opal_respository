using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_ForumThreadRevisionConfiguration : BaseEntityTypeConfiguration<CSL_ForumThreadRevision>
    {
        public override void Configure(EntityTypeBuilder<CSL_ForumThreadRevision> builder)
        {
            builder.ToTable("csl_ForumThreadRevision", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.HasOne(d => d.ForumThread)
                .WithMany(p => p.CslForumThreadRevision)
                .HasForeignKey(d => d.ForumThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Forum__Forum__167A2832");
        }
    }
}
