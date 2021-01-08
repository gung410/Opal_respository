using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_ParticipantAssignmentTrackQuizAnswerConfiguration : BaseEntityTypeConfiguration<CAM_ParticipantAssignmentTrackQuizAnswer>
    {
        public override void Configure(EntityTypeBuilder<CAM_ParticipantAssignmentTrackQuizAnswer> builder)
        {
            builder.HasKey(e => e.ParticipantAssignmentTrackId);

            builder.ToTable("cam_ParticipantAssignmentTrackQuizAnswer", "staging");

            builder.Property(e => e.ParticipantAssignmentTrackId).ValueGeneratedNever();

            builder.HasOne(d => d.ParticipantAssignmentTrack)
                .WithOne(p => p.CamParticipantAssignmentTrackQuizAnswer)
                .HasForeignKey<CAM_ParticipantAssignmentTrackQuizAnswer>(d => d.ParticipantAssignmentTrackId);

            builder.HasOne(d => d.QuizAssignmentForm)
                .WithMany(p => p.CamParticipantAssignmentTrackQuizAnswer)
                .HasForeignKey(d => d.QuizAssignmentFormId);
        }
    }
}
