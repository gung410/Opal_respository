using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_PollOptionsConfiguration : BaseEntityTypeConfiguration<CSL_PollOptions>
    {
        public override void Configure(EntityTypeBuilder<CSL_PollOptions> builder)
        {
            builder.ToTable("csl_PollOptions", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Answer)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Poll)
                .WithMany(p => p.CslPollOptions)
                .HasForeignKey(d => d.PollId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_PollO__PollI__2D729C23");
        }
    }
}
