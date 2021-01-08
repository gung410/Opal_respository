using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_QuizAssignmentFormQuestionConfiguration : BaseEntityTypeConfiguration<CAM_QuizAssignmentFormQuestion>
    {
        public override void Configure(EntityTypeBuilder<CAM_QuizAssignmentFormQuestion> builder)
        {
            builder.HasKey(e => e.QuizAssignmentFormQuestionId);

            builder.ToTable("cam_QuizAssignmentFormQuestion", "staging");

            builder.Property(e => e.QuizAssignmentFormQuestionId).ValueGeneratedNever();

            builder.Property(e => e.QuestionAnswerExplanatoryNote).HasColumnName("Question_AnswerExplanatoryNote");

            builder.Property(e => e.QuestionCorrectAnswer).HasColumnName("Question_CorrectAnswer");

            builder.Property(e => e.QuestionFeedbackCorrectAnswer).HasColumnName("Question_FeedbackCorrectAnswer");

            builder.Property(e => e.QuestionFeedbackWrongAnswer).HasColumnName("Question_FeedbackWrongAnswer");

            builder.Property(e => e.QuestionHint).HasColumnName("Question_Hint");

            builder.Property(e => e.QuestionOptions).HasColumnName("Question_Options");

            builder.Property(e => e.QuestionTitle)
                .HasColumnName("Question_Title")
                .HasColumnType("ntext");

            builder.Property(e => e.QuestionType)
                .IsRequired()
                .HasColumnName("Question_Type")
                .HasMaxLength(30)
                .IsUnicode(false);

            builder.HasOne(d => d.QuizAssignmentForm)
                .WithMany(p => p.CamQuizAssignmentFormQuestion)
                .HasForeignKey(d => d.QuizAssignmentFormId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
