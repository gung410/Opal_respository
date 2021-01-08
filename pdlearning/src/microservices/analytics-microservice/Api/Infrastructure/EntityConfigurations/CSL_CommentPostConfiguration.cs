using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_CommentPostConfiguration : BaseEntityTypeConfiguration<CSL_CommentPost>
    {
        public override void Configure(EntityTypeBuilder<CSL_CommentPost> builder)
        {
            builder.ToTable("csl_CommentPost", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Post)
                .WithMany(p => p.CslCommentPost)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Comme__PostI__42C2BEC4");
        }
    }
}
