using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CCPM_FormCommentConfiguration : BaseEntityTypeConfiguration<CCPM_FormComment>
    {
        public override void Configure(EntityTypeBuilder<CCPM_FormComment> builder)
        {
            builder.HasKey(e => e.FormCommentId);

            builder.ToTable("ccpm_FormComment", "staging");

            builder.Property(e => e.FormCommentId).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Form)
                .WithMany(p => p.CcpmFormComment)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
