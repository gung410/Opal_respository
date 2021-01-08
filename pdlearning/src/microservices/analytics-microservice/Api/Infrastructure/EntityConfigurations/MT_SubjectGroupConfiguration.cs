using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_SubjectGroupConfiguration : BaseEntityTypeConfiguration<MT_SubjectGroup>
    {
        public override void Configure(EntityTypeBuilder<MT_SubjectGroup> builder)
        {
            builder.HasKey(e => e.SubjectGroupId);

            builder.ToTable("mt_SubjectGroup", "staging");

            builder.Property(e => e.SubjectGroupId)
                .HasColumnName("SubjectGroupID")
                .ValueGeneratedNever();

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.SubjectId).HasColumnName("SubjectID");

            builder.HasOne(d => d.Subject)
                .WithMany(p => p.MtSubjectGroup)
                .HasForeignKey(d => d.SubjectId);
        }
    }
}
