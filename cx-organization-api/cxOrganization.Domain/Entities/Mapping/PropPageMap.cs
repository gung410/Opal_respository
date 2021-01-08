using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class PropPageMap.
    /// </summary>
    public class PropPageMap : IEntityTypeConfiguration<PropPageEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropPageMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<PropPageEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.PropPageId);

            // Properties
            // Table & Column Mappings
            builder.ToTable("PropPage", "prop");
            builder.Property(t => t.PropPageId).HasColumnName("PropPageID");
            builder.Property(t => t.TableTypeId).HasColumnName("TableTypeID");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Created).HasColumnName("Created");
        }
    }
}
