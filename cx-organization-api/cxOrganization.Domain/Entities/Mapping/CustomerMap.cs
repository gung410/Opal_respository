using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class CustomerMap.
    /// </summary>
    public class CustomerMap : IEntityTypeConfiguration<CustomerEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<CustomerEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.CustomerId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            builder.ToTable("Customer", "org");
            builder.Property(t => t.CustomerId).HasColumnName("CustomerID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Status).HasColumnName("Status");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.RootMenuId).HasColumnName("RootMenuID");
            builder.Property(t => t.HasUserIntegration).HasColumnName("HasUserIntegration");
            builder.Property(t => t.CodeName).HasColumnName("CodeName");
            builder.Property(t => t.Logo).HasColumnName("Logo");
            builder.Property(t => t.CssVariables).HasColumnName("CssVariables");
            builder.Property(t => t.Favicon).HasColumnName("Favicon");
        }
    }
}