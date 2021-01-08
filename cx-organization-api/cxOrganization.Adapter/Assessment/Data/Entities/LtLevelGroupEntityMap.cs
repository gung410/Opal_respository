using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class LtLevelGroupEntityMap : IEntityTypeConfiguration<LtLevelGroupEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LT_LevelGroupMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtLevelGroupEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.LanguageId, t.LevelGroupId });

            // Properties
            builder.Property(t => t.LanguageId)
                .ValueGeneratedNever();

            builder.Property(t => t.LevelGroupId)
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_LevelGroup", "at");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.LevelGroupId).HasColumnName("LevelGroupID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            builder.HasOne(t => t.LevelGroup)
                .WithMany(t => t.LT_LevelGroups)
                .HasForeignKey(d => d.LevelGroupId);

        }
    }
}
