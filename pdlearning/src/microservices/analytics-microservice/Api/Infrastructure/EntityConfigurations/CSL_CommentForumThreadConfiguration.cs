using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_CommentForumThreadConfiguration : BaseEntityTypeConfiguration<CSL_CommentForumThread>
    {
        public override void Configure(EntityTypeBuilder<CSL_CommentForumThread> builder)
        {
            builder.ToTable("csl_Comment_ForumThread", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.ForumThread)
                .WithMany(p => p.CslCommentForumThread)
                .HasForeignKey(d => d.ForumThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Comme__Forum__52842541");
        }
    }
}
