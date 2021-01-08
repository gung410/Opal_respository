using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_UserFollowPollConfiguration : BaseEntityTypeConfiguration<CSL_UserFollowPoll>
    {
        public override void Configure(EntityTypeBuilder<CSL_UserFollowPoll> builder)
        {
            builder.ToTable("csl_UserFollowPoll", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Poll)
                .WithMany(p => p.CslUserFollowPoll)
                .HasForeignKey(d => d.PollId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_UserF__PollI__776B912D");
        }
    }
}
