using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_UserFollowForumThreadConfiguration : BaseEntityTypeConfiguration<CSL_UserFollowForumThread>
    {
        public override void Configure(EntityTypeBuilder<CSL_UserFollowForumThread> builder)
        {
            builder.ToTable("csl_UserFollowForumThread", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.ForumThread)
                .WithMany(p => p.CslUserFollowForumThread)
                .HasForeignKey(d => d.ForumThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
