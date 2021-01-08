using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class DepartmentTypeMap.
    /// </summary>
    public class DepartmentTypeMap : IEntityTypeConfiguration<DepartmentTypeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<DepartmentTypeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.DepartmentTypeId);

            // Properties
            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("DepartmentType", "org");
            builder.Property(t => t.DepartmentTypeId).HasColumnName("DepartmentTypeID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.ArchetypeId).HasColumnName("ArchetypeID");
            builder.Property(t => t.ParentId).HasColumnName("ParentID");
        }
    }
}