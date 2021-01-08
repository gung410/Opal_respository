using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_CommentPollConfiguration : BaseEntityTypeConfiguration<CSL_CommentPoll>
    {
        public override void Configure(EntityTypeBuilder<CSL_CommentPoll> builder)
        {
            builder.ToTable("csl_Comment_Poll", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Poll)
                .WithMany(p => p.CslCommentPoll)
                .HasForeignKey(d => d.PollId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Comme__PollI__19D5B7CA");
        }
    }
}
