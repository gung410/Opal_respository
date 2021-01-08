using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CCPM_DigitalContentCommentsConfiguration : BaseEntityTypeConfiguration<CCPM_DigitalContentComments>
    {
        public override void Configure(EntityTypeBuilder<CCPM_DigitalContentComments> builder)
        {
            builder.HasKey(e => e.DigitalContentCommentId);

            builder.ToTable("ccpm_DigitalContent_Comments", "staging");

            builder.Property(e => e.DigitalContentCommentId)
                .HasColumnName("DigitalContent_CommentId")
                .ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.DigitalContent)
                .WithMany(p => p.CcpmDigitalContentComments)
                .HasForeignKey(d => d.DigitalContentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
