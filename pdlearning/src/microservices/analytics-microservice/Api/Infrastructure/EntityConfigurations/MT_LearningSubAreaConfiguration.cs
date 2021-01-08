using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class MT_LearningSubAreaConfiguration : BaseEntityTypeConfiguration<MT_LearningSubArea>
    {
        public override void Configure(EntityTypeBuilder<MT_LearningSubArea> builder)
        {
            builder.HasKey(e => e.LearningSubAreaId);

            builder.ToTable("mt_LearningSubArea", "staging");

            builder.Property(e => e.LearningSubAreaId)
                .HasColumnName("LearningSubAreaID")
                .ValueGeneratedNever();

            builder.Property(e => e.CodingScheme).HasMaxLength(512);

            builder.Property(e => e.DisplayText).HasMaxLength(512);

            builder.Property(e => e.FullStatement).HasMaxLength(512);

            builder.Property(e => e.GroupCode).HasMaxLength(512);

            builder.Property(e => e.LearningAreaId).HasColumnName("LearningAreaID");

            builder.Property(e => e.Note).HasMaxLength(4000);

            builder.Property(e => e.Type).HasMaxLength(100);

            builder.HasOne(d => d.LearningArea)
                .WithMany(p => p.MtLearningSubArea)
                .HasForeignKey(d => d.LearningAreaId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
