using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_LearningAreaConfiguration : BaseEntityTypeConfiguration<MT_LearningArea>
    {
        public override void Configure(EntityTypeBuilder<MT_LearningArea> builder)
        {
            builder.HasKey(e => e.LearningAreaId);

            builder.ToTable("mt_LearningArea", "staging");

            builder.Property(e => e.LearningAreaId)
                .HasColumnName("LearningAreaID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.LearningDimensionId).HasColumnName("LearningDimensionID");

            builder.Property(e => e.Note).HasMaxLength(4000);

            builder.Property(e => e.Type).HasMaxLength(100);

            builder.HasOne(d => d.LearningDimension)
                .WithMany(p => p.MtLearningArea)
                .HasForeignKey(d => d.LearningDimensionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
