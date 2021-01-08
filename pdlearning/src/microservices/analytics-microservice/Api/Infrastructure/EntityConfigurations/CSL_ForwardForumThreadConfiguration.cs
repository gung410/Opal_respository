using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_ForwardForumThreadConfiguration : BaseEntityTypeConfiguration<CSL_ForwardForumThread>
    {
        public override void Configure(EntityTypeBuilder<CSL_ForwardForumThread> builder)
        {
            builder.ToTable("csl_Forward_ForumThread", "staging");

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.ForumThread)
                .WithMany(p => p.CslForwardForumThread)
                .HasForeignKey(d => d.ForumThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
