using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_SubjectConfiguration : BaseEntityTypeConfiguration<MT_Subject>
    {
        public override void Configure(EntityTypeBuilder<MT_Subject> builder)
        {
            builder.HasKey(e => e.SubjectId);

            builder.ToTable("mt_Subject", "staging");

            builder.Property(e => e.SubjectId)
                .HasColumnName("SubjectID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.Note).HasMaxLength(4000);

            builder.Property(e => e.ServiceSchemeId).HasColumnName("ServiceSchemeID");

            builder.Property(e => e.Type).HasMaxLength(100);

            builder.HasOne(d => d.ServiceScheme)
                .WithMany(p => p.MtSubject)
                .HasForeignKey(d => d.ServiceSchemeId);
        }
    }
}
