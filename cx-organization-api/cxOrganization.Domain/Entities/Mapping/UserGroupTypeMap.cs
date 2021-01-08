using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class UserGroupTypeMap.
    /// </summary>
    public class UserGroupTypeMap : IEntityTypeConfiguration<UserGroupTypeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<UserGroupTypeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.UserGroupTypeId);

            // Properties
            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("UserGroupType","org");
            builder.Property(t => t.UserGroupTypeId).HasColumnName("UserGroupTypeID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Created).HasColumnName("Created");
        }
    }
}
