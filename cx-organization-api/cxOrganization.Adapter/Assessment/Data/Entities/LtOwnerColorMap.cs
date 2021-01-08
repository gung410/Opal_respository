using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class LtOwnerColorMap : IEntityTypeConfiguration<LtOwnerColorEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtOwnerColorMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtOwnerColorEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.OwnerColorId, t.LanguageId });

            // Properties
            builder.Property(t => t.OwnerColorId)
                .ValueGeneratedNever();

            builder.Property(t => t.LanguageId)
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("LT_OwnerColor", "at");
            builder.Property(t => t.OwnerColorId).HasColumnName("OwnerColorID");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Created).HasColumnName("Created");

            // Relationships
            builder.HasOne(t => t.OwnerColor)
                .WithMany(t => t.LT_OwnerColor)
                .HasForeignKey(d => d.OwnerColorId);
        }
    }
}
