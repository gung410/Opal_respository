using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_SubjectKeyWordsConfiguration : BaseEntityTypeConfiguration<MT_SubjectKeyWords>
    {
        public override void Configure(EntityTypeBuilder<MT_SubjectKeyWords> builder)
        {
            builder.HasKey(e => e.SubjectKeyWordId);

            builder.ToTable("mt_SubjectKeyWords", "staging");

            builder.Property(e => e.SubjectKeyWordId)
                .HasColumnName("SubjectKeyWordID")
                .ValueGeneratedNever();

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.SubjectGroupId).HasColumnName("SubjectGroupID");

            builder.HasOne(d => d.SubjectGroup)
                .WithMany(p => p.MtSubjectKeyWords)
                .HasForeignKey(d => d.SubjectGroupId);
        }
    }
}
