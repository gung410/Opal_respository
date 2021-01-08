using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class LtLevelLimitEntityMap : IEntityTypeConfiguration<LtLevelLimitEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LT_LevelLimitMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtLevelLimitEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.LevelLimitId, t.LanguageId });

            // Properties
            builder.Property(t => t.LevelLimitId)
                .ValueGeneratedNever();

            builder.Property(t => t.LanguageId)
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_LevelLimit", "at");
            builder.Property(t => t.LevelLimitId).HasColumnName("LevelLimitID");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            builder.HasOne(t => t.LevelLimit)
                .WithMany(t => t.LT_LevelLimits)
                .HasForeignKey(d => d.LevelLimitId);
        }
    }
}
