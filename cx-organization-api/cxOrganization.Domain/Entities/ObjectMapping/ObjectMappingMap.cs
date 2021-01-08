using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class ObjectMappingMap.
    /// </summary>
    public class ObjectMappingMap : IEntityTypeConfiguration<ObjectMappingEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectMappingMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<ObjectMappingEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.OMId);

            // Properties
            // Table & Column Mappings
            builder.ToTable("ObjectMapping", "dbo");
            builder.Property(t => t.OMId).HasColumnName("OMID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.FromTableTypeId).HasColumnName("FromTableTypeID");
            builder.Property(t => t.FromId).HasColumnName("FromID");
            builder.Property(t => t.ToTableTypeId).HasColumnName("ToTableTypeID");
            builder.Property(t => t.ToId).HasColumnName("ToID");
            builder.Property(t => t.RelationTypeId).HasColumnName("RelationTypeID");
        }
    }
}