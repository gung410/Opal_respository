using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class PropOptionMap.
    /// </summary>
    public class PropOptionMap : IEntityTypeConfiguration<PropOptionEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropOptionMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<PropOptionEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.PropOptionId);

            // Properties
            // Table & Column Mappings
            builder.ToTable("PropOption", "prop");
            builder.Property(t => t.PropOptionId).HasColumnName("PropOptionID");
            builder.Property(t => t.PropertyId).HasColumnName("PropertyID");
            builder.Property(t => t.Value).HasColumnName("Value");

        }
    }
}
