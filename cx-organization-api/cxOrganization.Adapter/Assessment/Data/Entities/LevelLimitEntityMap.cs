using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class LevelLimitEntityMap : IEntityTypeConfiguration<LevelLimitEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelLimitEntityMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LevelLimitEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.LevelLimitId);

            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            // Properties

            // Table & Column Mappings
            builder.ToTable("LevelLimit", "at");
            builder.Property(t => t.LevelLimitId).HasColumnName("LevelLimitID");
            builder.Property(t => t.AlternativeId).HasColumnName("AlternativeID");
            builder.Property(t => t.LevelGroupId).HasColumnName("LevelGroupID");
            builder.Property(t => t.CategoryId).HasColumnName("CategoryID");
            builder.Property(t => t.QuestionId).HasColumnName("QuestionID");
            builder.Property(t => t.MinValue).HasColumnName("MinValue");
            builder.Property(t => t.MaxValue).HasColumnName("MaxValue");
            builder.Property(t => t.Sigchange).HasColumnName("Sigchange");
            builder.Property(t => t.OwnerColorId).HasColumnName("OwnerColorID");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.NegativeTrend).HasColumnName("NegativeTrend");
            builder.Property(t => t.ItemId).HasColumnName("ItemID");
            builder.Property(t => t.MatchingType).HasColumnName("MatchingType");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");

            // Relationships
            builder.HasOne(t => t.LevelGroup)
                .WithMany(t => t.LevelLimits)
                .HasForeignKey(d => d.LevelGroupId);

            builder.HasOne(t => t.OwnerColor)
                .WithMany(t => t.LevelLimits)
                .HasForeignKey(d => d.OwnerColorId);
        }
    }
}
