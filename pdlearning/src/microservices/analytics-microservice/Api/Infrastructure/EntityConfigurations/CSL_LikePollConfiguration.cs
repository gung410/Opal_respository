using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_LikePollConfiguration : BaseEntityTypeConfiguration<CSL_LikePoll>
    {
        public override void Configure(EntityTypeBuilder<CSL_LikePoll> builder)
        {
            builder.ToTable("csl_Like_Poll", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Poll)
                .WithMany(p => p.CslLikePoll)
                .HasForeignKey(d => d.PollId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_Like___PollI__16F94B1F");
        }
    }
}
