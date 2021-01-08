using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class ActivityMap.
    /// </summary>
    public class ActivityMap : IEntityTypeConfiguration<ActivityEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<ActivityEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.ActivityID);

            // Properties
            builder.Property(t => t.StyleSheet)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.OLAPServer)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.OLAPDB)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.ExtID)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("Activity", "at");
            builder.Property(t => t.ActivityID).HasColumnName("ActivityID");
            builder.Property(t => t.LanguageID).HasColumnName("LanguageID");
            builder.Property(t => t.OwnerID).HasColumnName("OwnerID");
            builder.Property(t => t.StyleSheet).HasColumnName("StyleSheet");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.TooltipType).HasColumnName("TooltipType");
            builder.Property(t => t.Listview).HasColumnName("Listview");
            builder.Property(t => t.Descview).HasColumnName("Descview");
            builder.Property(t => t.Chartview).HasColumnName("Chartview");
            builder.Property(t => t.OLAPServer).HasColumnName("OLAPServer");
            builder.Property(t => t.OLAPDB).HasColumnName("OLAPDB");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.SelectionHeaderType).HasColumnName("SelectionHeaderType");
            builder.Property(t => t.ExtID).HasColumnName("ExtID");
            builder.Property(t => t.UseOLAP).HasColumnName("UseOLAP");
            builder.Property(t => t.ArchetypeId).HasColumnName("ArchetypeID");
        }
    }
}