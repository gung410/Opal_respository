using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class ScaleMap.
    /// </summary>
    public class ScaleMap : IEntityTypeConfiguration<ScaleEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<ScaleEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.ScaleID);

            // Properties
            builder.Property(t => t.Tag)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            builder.ToTable("Scale", "at");
            builder.Property(t => t.ScaleID).HasColumnName("ScaleID");
            builder.Property(t => t.ActivityID).HasColumnName("ActivityID");
            builder.Property(t => t.MinSelect).HasColumnName("MinSelect");
            builder.Property(t => t.MaxSelect).HasColumnName("MaxSelect");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.MinValue).HasColumnName("MinValue");
            builder.Property(t => t.MaxValue).HasColumnName("MaxValue");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.Tag).HasColumnName("Tag");
            builder.Property(t => t.ExtID).HasColumnName("ExtID");
        }
    }
}