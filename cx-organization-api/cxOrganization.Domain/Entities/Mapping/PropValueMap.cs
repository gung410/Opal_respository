using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class PropValueMap.
    /// </summary>
    public class PropValueMap : IEntityTypeConfiguration<PropValueEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropValueMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<PropValueEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.PropValueId);

            // Properties
            builder.Property(t => t.Value)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("PropValue", "prop");
            builder.Property(t => t.PropValueId).HasColumnName("PropValueID");
            builder.Property(t => t.PropertyId).HasColumnName("PropertyID");
            builder.Property(t => t.PropOptionId).HasColumnName("PropOptionID");
            builder.Property(t => t.ItemId).HasColumnName("ItemID");
            builder.Property(t => t.Value).HasColumnName("Value");
            builder.Property(t => t.PropFileId).HasColumnName("PropFileID");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            builder.Property(t => t.Updated).HasColumnName("Updated");
            builder.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            builder.Property(t => t.CustomerId).HasColumnName("CustomerID");
        }
    }
}