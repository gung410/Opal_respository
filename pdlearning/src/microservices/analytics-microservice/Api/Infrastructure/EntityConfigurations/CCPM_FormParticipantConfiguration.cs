using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CCPM_FormParticipantConfiguration : BaseEntityTypeConfiguration<CCPM_FormParticipant>
    {
        public override void Configure(EntityTypeBuilder<CCPM_FormParticipant> builder)
        {
            builder.HasKey(e => e.FormParticipantId);

            builder.ToTable("ccpm_FormParticipant", "staging");

            builder.Property(e => e.FormParticipantId).ValueGeneratedNever();

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);

            builder.HasOne(d => d.Form)
                .WithMany(p => p.CcpmFormParticipant)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
