using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_ParticipantAssignmentTrackQuizQuestionAnswerConfiguration : BaseEntityTypeConfiguration<CAM_ParticipantAssignmentTrackQuizQuestionAnswer>
    {
        public override void Configure(EntityTypeBuilder<CAM_ParticipantAssignmentTrackQuizQuestionAnswer> builder)
        {
            builder.HasKey(e => e.ParticipantAssignmentTrackQuizQuestionAnswerId);

            builder.ToTable("cam_ParticipantAssignmentTrackQuizQuestionAnswer", "staging");

            builder.Property(e => e.ParticipantAssignmentTrackQuizQuestionAnswerId).ValueGeneratedNever();

            builder.HasOne(d => d.ParticipantAssignmentTrack)
                .WithMany(p => p.CamParticipantAssignmentTrackQuizQuestionAnswer)
                .HasForeignKey(d => d.ParticipantAssignmentTrackId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.QuizAssignmentFormQuestion)
                .WithMany(p => p.CamParticipantAssignmentTrackQuizQuestionAnswer)
                .HasForeignKey(d => d.QuizAssignmentFormQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
