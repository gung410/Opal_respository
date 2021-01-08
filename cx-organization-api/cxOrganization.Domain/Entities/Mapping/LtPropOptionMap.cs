using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LtPropOptionMap.
    /// </summary>
    public class LtPropOptionMap : IEntityTypeConfiguration<LtPropOptionEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtPropOptionMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtPropOptionEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.LanguageId, t.PropOptionId });

            // Properties

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_PropOption", "prop");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.PropOptionId).HasColumnName("PropOptionID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");
            
            builder.HasOne(t => t.PropOption)
                .WithMany(t => t.LtPropOptions)
                .HasForeignKey(d => d.PropOptionId);
        }
    }
}
