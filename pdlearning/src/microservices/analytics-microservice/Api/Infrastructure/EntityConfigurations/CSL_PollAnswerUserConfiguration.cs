using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CSL_PollAnswerUserConfiguration : BaseEntityTypeConfiguration<CSL_PollAnswerUser>
    {
        public override void Configure(EntityTypeBuilder<CSL_PollAnswerUser> builder)
        {
            builder.ToTable("csl_PollAnswerUser", "staging");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Poll)
                .WithMany(p => p.CslPollAnswerUser)
                .HasForeignKey(d => d.PollId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_PollA__PollI__39D87308");

            builder.HasOne(d => d.PollOption)
                .WithMany(p => p.CslPollAnswerUser)
                .HasForeignKey(d => d.PollOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__csl_PollA__PollO__3ACC9741");
        }
    }
}
