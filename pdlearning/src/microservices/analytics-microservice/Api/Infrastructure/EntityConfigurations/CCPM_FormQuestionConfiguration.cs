using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CCPM_FormQuestionConfiguration : BaseEntityTypeConfiguration<CCPM_FormQuestion>
    {
        public override void Configure(EntityTypeBuilder<CCPM_FormQuestion> builder)
        {
            builder.HasKey(e => e.FormQuestionsId);

            builder.ToTable("ccpm_FormQuestion", "staging");

            builder.Property(e => e.FormQuestionsId).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.QuestionType).HasMaxLength(50);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Form)
                .WithMany(p => p.CcpmFormQuestion)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
