using System.Text.Json;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class ParticipantAssignmentTrackQuizQuestionAnswerConfiguration : BaseEntityTypeConfiguration<ParticipantAssignmentTrackQuizQuestionAnswer>
    {
        public override void Configure(EntityTypeBuilder<ParticipantAssignmentTrackQuizQuestionAnswer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasOne(x => x.QuizAnswer).WithMany(x => x.QuestionAnswers).HasForeignKey(x => x.QuizAnswerId);
            builder
                .Property(p => p.AnswerValue)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<object>(v, null) : null);

            builder.HasIndex(x => x.QuizAnswerId);
            builder.HasIndex(x => x.QuizAssignmentFormQuestionId);
            builder.HasIndex(x => x.ManualScoredBy);
            builder.HasIndex(x => x.SubmittedDate);
            builder.HasIndex(x => x.IsDeleted);
        }
    }
}
