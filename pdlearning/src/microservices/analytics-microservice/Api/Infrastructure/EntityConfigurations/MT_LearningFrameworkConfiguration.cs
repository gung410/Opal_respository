using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_LearningFrameworkConfiguration : BaseEntityTypeConfiguration<MT_LearningFramework>
    {
        public override void Configure(EntityTypeBuilder<MT_LearningFramework> builder)
        {
            builder.HasKey(e => e.LearningFrameworkId);

            builder.ToTable("mt_LearningFramework", "staging");

            builder.Property(e => e.LearningFrameworkId)
                .HasColumnName("LearningFrameworkID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.Note).HasMaxLength(4000);

            builder.Property(e => e.ServiceSchemeId).HasColumnName("ServiceSchemeID");

            builder.Property(e => e.Type).HasMaxLength(100);

            builder.HasOne(d => d.ServiceScheme)
                .WithMany(p => p.MtLearningFramework)
                .HasForeignKey(d => d.ServiceSchemeId);
        }
    }
}
