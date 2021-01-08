using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_TeachingSubjectConfiguration : BaseEntityTypeConfiguration<MT_TeachingSubject>
    {
        public override void Configure(EntityTypeBuilder<MT_TeachingSubject> builder)
        {
            builder.HasKey(e => e.TeachingSubjectId);

            builder.ToTable("mt_TeachingSubject", "staging");

            builder.Property(e => e.TeachingSubjectId)
                .HasColumnName("TeachingSubjectID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(1000);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.Note).HasMaxLength(4000);

            builder.Property(e => e.Type).HasMaxLength(100);
        }
    }
}
