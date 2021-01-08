using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CCPM_FormAnswerConfiguration : BaseEntityTypeConfiguration<CCPM_FormAnswer>
    {
        public override void Configure(EntityTypeBuilder<CCPM_FormAnswer> builder)
        {
            builder.HasKey(e => e.FormAnswerId);

            builder.ToTable("ccpm_FormAnswer", "staging");

            builder.Property(e => e.FormAnswerId).ValueGeneratedNever();

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.UpdatedByDepartmentId).HasMaxLength(64);

            builder.HasOne(d => d.Form)
                .WithMany(p => p.CcpmFormAnswer)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.CcpmFormAnswer)
                .HasForeignKey(d => d.UserHistoryId);
        }
    }
}
