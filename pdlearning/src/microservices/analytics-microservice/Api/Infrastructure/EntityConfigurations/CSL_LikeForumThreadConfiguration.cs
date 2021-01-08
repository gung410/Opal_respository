using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_LikeForumThreadConfiguration : BaseEntityTypeConfiguration<CSL_LikeForumThread>
    {
        public override void Configure(EntityTypeBuilder<CSL_LikeForumThread> builder)
        {
            builder.ToTable("csl_Like_ForumThread", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.HasOne(d => d.ForumThread)
                .WithMany(p => p.CslLikeForumThread)
                .HasForeignKey(d => d.ForumThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
