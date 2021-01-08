using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    /// <summary>
    /// Class AlternativeMap.
    /// </summary>
    public class AlternativeMap : IEntityTypeConfiguration<AlternativeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlternativeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<AlternativeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.AlternativeID);

            // Properties
            builder.Property(t => t.Format)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.CssClass)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.DefaultValue)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.Tag)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.ExtID)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Width)
                .IsRequired()
                .HasMaxLength(32);

            // Table & Column Mappings
            builder.ToTable("Alternative", "at");
            builder.Property(t => t.AlternativeID).HasColumnName("AlternativeID");
            builder.Property(t => t.ScaleID).HasColumnName("ScaleID");
            builder.Property(t => t.AGID).HasColumnName("AGID");
            builder.Property(t => t.Type).HasColumnName("Type");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Value).HasColumnName("Value");
            builder.Property(t => t.InvertedValue).HasColumnName("InvertedValue");
            builder.Property(t => t.Calc).HasColumnName("Calc");
            builder.Property(t => t.SC).HasColumnName("SC");
            builder.Property(t => t.MinValue).HasColumnName("MinValue");
            builder.Property(t => t.MaxValue).HasColumnName("MaxValue");
            builder.Property(t => t.Format).HasColumnName("Format");
            builder.Property(t => t.Size).HasColumnName("Size");
            builder.Property(t => t.CssClass).HasColumnName("CssClass");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.DefaultValue).HasColumnName("DefaultValue");
            builder.Property(t => t.Tag).HasColumnName("Tag");
            builder.Property(t => t.ExtID).HasColumnName("ExtID");
            builder.Property(t => t.Width).HasColumnName("Width");
            builder.Property(t => t.ParentID).HasColumnName("ParentID");
            builder.Property(t => t.OwnerColorID).HasColumnName("OwnerColorID");
            builder.Property(t => t.DefaultCalcType).HasColumnName("DefaultCalcType");
            builder.Property(t => t.UseEncryption).HasColumnName("UseEncryption");

            // Relationships
            builder.HasOne(t => t.Scale)
                .WithMany(t => t.Alternatives)
                .HasForeignKey(d => d.ScaleID);
        }
    }
}