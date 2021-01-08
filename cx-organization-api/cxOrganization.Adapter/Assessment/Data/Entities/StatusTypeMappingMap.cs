using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class StatusTypeMappingMap.
    /// </summary>
    public class StatusTypeMappingMap : IEntityTypeConfiguration<StatusTypeMappingEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusTypeMappingMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<StatusTypeMappingEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.StatusTypeMappingID);

            // Properties
            // Table & Column Mappings
            builder.ToTable("StatusTypeMapping", "at");
            builder.Property(t => t.StatusTypeMappingID).HasColumnName("StatusTypeMappingID");
            builder.Property(t => t.ActivityID).HasColumnName("ActivityID");
            builder.Property(t => t.CustomerID).HasColumnName("CustomerID");
            builder.Property(t => t.FromStatusTypeID).HasColumnName("FromStatusTypeID");
            builder.Property(t => t.ToStatusTypeID).HasColumnName("ToStatusTypeID");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.ShowActionInUI).HasColumnName("ShowActionInUI");
            builder.Property(t => t.Settings).HasColumnName("Settings");
        }
    }
}
