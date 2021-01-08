using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class LtAlternativeMap : IEntityTypeConfiguration<LtAlternativeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtAlternativeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtAlternativeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.LanguageID, t.AlternativeID });

            // Properties
            builder.Property(t => t.LanguageID)
                .ValueGeneratedNever();

            builder.Property(t => t.AlternativeID)
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Label)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_Alternative", "at");
            builder.Property(t => t.LanguageID).HasColumnName("LanguageID");
            builder.Property(t => t.AlternativeID).HasColumnName("AlternativeID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Label).HasColumnName("Label");
            builder.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            builder.HasOne(t => t.Alternative)
                .WithMany(t => t.LtAlternativeEntities)
                .HasForeignKey(d => d.AlternativeID);
        }
    }
}
