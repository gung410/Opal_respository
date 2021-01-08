using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CCPM_FormQuestionAnswerConfiguration : BaseEntityTypeConfiguration<CCPM_FormQuestionAnswer>
    {
        public override void Configure(EntityTypeBuilder<CCPM_FormQuestionAnswer> builder)
        {
            builder.HasKey(e => e.FormQuestionAnswerId);

            builder.ToTable("ccpm_FormQuestionAnswer", "staging");

            builder.Property(e => e.FormQuestionAnswerId).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.ScoredByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.FormAnswer)
                .WithMany(p => p.CcpmFormQuestionAnswer)
                .HasForeignKey(d => d.FormAnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.FormQuestion)
                .WithMany(p => p.CcpmFormQuestionAnswer)
                .HasForeignKey(d => d.FormQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
