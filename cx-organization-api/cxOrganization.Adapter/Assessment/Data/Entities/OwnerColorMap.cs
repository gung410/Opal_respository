using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Adapter.Assessment.Data.Entities
{
    public class OwnerColorMap : IEntityTypeConfiguration<OwnerColorEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerColorMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<OwnerColorEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.OwnerColorId);

            // Properties
            builder.Property(t => t.FriendlyName)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.FillColor)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(t => t.TextColor)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(t => t.BorderColor)
                .IsRequired()
                .HasMaxLength(16);

            // Table & Column Mappings
            builder.ToTable("OwnerColor", "at");
            builder.Property(t => t.OwnerColorId).HasColumnName("OwnerColorID");
            builder.Property(t => t.ThemeID).HasColumnName("ThemeID");
            builder.Property(t => t.OwnerID).HasColumnName("OwnerID");
            builder.Property(t => t.CustomerID).HasColumnName("CustomerID");
            builder.Property(t => t.FriendlyName).HasColumnName("FriendlyName");
            builder.Property(t => t.FillColor).HasColumnName("FillColor");
            builder.Property(t => t.TextColor).HasColumnName("TextColor");
            builder.Property(t => t.BorderColor).HasColumnName("BorderColor");
            builder.Property(t => t.No).HasColumnName("No");
            builder.Property(t => t.Created).HasColumnName("Created");
        }
    }
}
