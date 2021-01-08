using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class HierarchyMap.
    /// </summary>
    public class HierarchyMap : IEntityTypeConfiguration<HierarchyEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<HierarchyEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.HierarchyId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("Hierarchy", "org");
            builder.Property(t => t.HierarchyId).HasColumnName("HierarchyID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.Deleted).HasColumnName("Deleted");
            builder.Property(t => t.Created).HasColumnName("Created");
        }
    }
}