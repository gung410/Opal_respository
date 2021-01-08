using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class StatusTypeMap.
    /// </summary>
    public class StatusTypeMap : IEntityTypeConfiguration<StatusTypeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<StatusTypeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.StatusTypeID);

            // Properties
            // Table & Column Mappings
            builder.ToTable("StatusType", "at");
            builder.Property(t => t.StatusTypeID).HasColumnName("StatusTypeID");
            builder.Property(t => t.OwnerID).HasColumnName("OwnerID");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Value).HasColumnName("Value");
            builder.Property(t => t.CodeName).HasColumnName("CodeName");
        }
    }
}