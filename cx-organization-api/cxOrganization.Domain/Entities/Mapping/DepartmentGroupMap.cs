using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class DepartmentGroupMap.
    /// </summary>
    public class DepartmentGroupMap : IEntityTypeConfiguration<DepartmentGroupEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentGroupMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<DepartmentGroupEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.DepartmentGroupId);

            // Properties
            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("DepartmentGroup", "org");
            builder.Property(t => t.DepartmentGroupId).HasColumnName("DepartmentGroupID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Created).HasColumnName("Created");
        }
    }
}