using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserActivityResumeConfiguration : BaseEntityTypeConfiguration<Learner_UserActivityResume>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserActivityResume> builder)
        {
            builder.ToTable("learner_UserActivity_Resume", "staging");

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Id)
                   .HasColumnName("UserActivityResumeId");

            builder.Property(e => e.ClientId)
                .HasColumnName("clientId")
                .HasMaxLength(64);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.LoginFromMobile).HasColumnName("loginFromMobile");

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.LearnerUserActivityResume)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
