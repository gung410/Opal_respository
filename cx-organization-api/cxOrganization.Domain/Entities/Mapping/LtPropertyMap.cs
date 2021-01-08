using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LtPropMap.
    /// </summary>
    public class LtPropertyMap : IEntityTypeConfiguration<LtPropertyEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtPropMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtPropertyEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.LanguageId, t.PropertyId });

            // Properties

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_Prop", "prop");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.PropertyId).HasColumnName("PropertyID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");
            
            builder.HasOne(t => t.Property)
                .WithMany(t => t.LtProperties)
                .HasForeignKey(d => d.PropertyId);
        }
    }
}
