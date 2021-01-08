using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class UserTypeMap.
    /// </summary>
    public class UserTypeMap : IEntityTypeConfiguration<UserTypeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<UserTypeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.UserTypeId);

            // Properties
            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("UserType", "org");
            builder.Property(t => t.UserTypeId).HasColumnName("UserTypeID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.ArchetypeId).HasColumnName("ArchetypeID");
            builder.Property(t => t.ParentId).HasColumnName("ParentID");
        }
    }
}