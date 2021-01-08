using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_ForwardPostConfiguration : BaseEntityTypeConfiguration<CSL_ForwardPost>
    {
        public override void Configure(EntityTypeBuilder<CSL_ForwardPost> builder)
        {
            builder.ToTable("csl_ForwardPost", "staging");

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Post)
                .WithMany(p => p.CslForwardPost)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Forwa__PostI__37510C18");
        }
    }
}
