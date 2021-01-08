using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserDigitalContentConfiguration : BaseEntityTypeConfiguration<Learner_UserDigitalContent>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserDigitalContent> builder)
        {
            builder.HasKey(e => e.UserDigitalContentId);

            builder.ToTable("learner_UserDigitalContent", "staging");

            builder.Property(e => e.UserDigitalContentId).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.DigitalContentType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.RateComment).HasMaxLength(2000);

            builder.Property(e => e.RateCommentTitle).HasMaxLength(100);

            builder.Property(e => e.ReviewStatus).HasMaxLength(1000);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.Version)
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.HasOne(d => d.Department)
                .WithMany(p => p.LearnerUserDigitalContent)
                .HasForeignKey(d => d.DepartmentId);

            builder.HasOne(d => d.DigitalContent)
                .WithMany(p => p.LearnerUserDigitalContent)
                .HasForeignKey(d => d.DigitalContentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.LearnerUserDigitalContent)
                .HasForeignKey(d => d.UserHistoryId);
        }
    }
}
