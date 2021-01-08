using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserActivityLandingPageConfiguration : BaseEntityTypeConfiguration<Learner_UserActivityLandingPage>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserActivityLandingPage> builder)
        {
            builder.ToTable("learner_UserActivity_LandingPage", "staging");

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.ActionName).HasMaxLength(100);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.LearnerUserActivityLandingPages)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
