using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserActivityRevisitMluConfiguration : BaseEntityTypeConfiguration<Learner_UserActivityRevisitMlu>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserActivityRevisitMlu> builder)
        {
            builder.ToTable("learner_UserActivity_RevisitMLU", "staging");

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.ActionName).HasMaxLength(100);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.VisitMode).HasMaxLength(100);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.LearnerUserActivityRevisitMlu)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
