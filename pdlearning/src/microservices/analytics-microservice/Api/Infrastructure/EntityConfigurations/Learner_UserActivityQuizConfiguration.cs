using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserActivityQuizConfiguration : BaseEntityTypeConfiguration<Learner_UserActivityQuiz>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserActivityQuiz> builder)
        {
            builder.ToTable("learner_UserActivity_Quiz", "staging");

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.ActionName).HasMaxLength(100);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.LearnerUserActivityQuizzes)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
