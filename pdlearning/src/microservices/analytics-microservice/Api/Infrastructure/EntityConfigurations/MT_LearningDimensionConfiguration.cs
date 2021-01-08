using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_LearningDimensionConfiguration : BaseEntityTypeConfiguration<MT_LearningDimension>
    {
        public override void Configure(EntityTypeBuilder<MT_LearningDimension> builder)
        {
            builder.HasKey(e => e.LearningDimensionId);

            builder.ToTable("mt_LearningDimension", "staging");

            builder.Property(e => e.LearningDimensionId)
                .HasColumnName("LearningDimensionID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.LearningFrameworkId).HasColumnName("LearningFrameworkID");

            builder.Property(e => e.Note).HasMaxLength(4000);

            builder.Property(e => e.Type).HasMaxLength(100);

            builder.HasOne(d => d.LearningFramework)
                .WithMany(p => p.MtLearningDimension)
                .HasForeignKey(d => d.LearningFrameworkId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
