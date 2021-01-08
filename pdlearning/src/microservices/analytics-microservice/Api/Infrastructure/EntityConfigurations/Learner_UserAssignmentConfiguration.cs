using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserAssignmentConfiguration : BaseEntityTypeConfiguration<Learner_UserAssignment>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserAssignment> builder)
        {
            builder.HasKey(e => e.UserAssignmentId);

            builder.ToTable("learner_UserAssignments", "staging");

            builder.Property(e => e.UserAssignmentId).ValueGeneratedNever();

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.HasOne(d => d.Assignment)
                .WithMany(p => p.LearnerUserAssignments)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.ParticipantAssignmentTrack)
                .WithMany(p => p.LearnerUserAssignments)
                .HasForeignKey(d => d.ParticipantAssignmentTrackId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Registration)
                .WithMany(p => p.LearnerUserAssignments)
                .HasForeignKey(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.LearnerUserAssignments)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
