using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class PropMap.
    /// </summary>
    public class PropertyMap : IEntityTypeConfiguration<PropertyEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<PropertyEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.PropertyId);

            // Properties
            builder.Property(t => t.FormatString)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Prop", "prop");
            builder.Property(t => t.PropertyId).HasColumnName("PropertyID");
            builder.Property(t => t.PropPageId).HasColumnName("PropPageID");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.ValueType).HasColumnName("ValueType");
            builder.Property(t => t.FormatString).HasColumnName("FormatString");
            builder.Property(t => t.MultiValue).HasColumnName("MultiValue");

            // Relationships
            builder.HasOne(t => t.PropPage)
                .WithMany(t => t.Properties)
                .HasForeignKey(d => d.PropPageId);
        }
    }
}
