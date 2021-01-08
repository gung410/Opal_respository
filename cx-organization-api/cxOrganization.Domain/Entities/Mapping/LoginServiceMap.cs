using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LoginServiceMap.
    /// </summary>
	public class LoginServiceMap : IEntityTypeConfiguration<LoginServiceEntity>
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginServiceMap"/> class.
        /// </summary>
		public void Configure(EntityTypeBuilder<LoginServiceEntity> builder)
        {
            // Primary Key
            // Primary Key
            builder.HasKey(t => t.LoginServiceID);
            // Properties
            builder.Property(t => t.ClientId).HasMaxLength(512);
            builder.Property(t => t.ClientSecret).HasMaxLength(512);
            builder.Property(t => t.IconUrl).HasMaxLength(512);
            builder.Property(t => t.Authority).HasMaxLength(512);
            builder.Property(t => t.PrimaryClaimType).IsRequired().HasMaxLength(512);
            builder.Property(t => t.Disabled).IsRequired();
            builder.Property(t => t.SecondaryClaimType).IsRequired().HasMaxLength(512);
            builder.Property(t => t.MetadataAddress).HasMaxLength(512);
            builder.Property(t => t.RedirectUri).HasMaxLength(512);
            builder.Property(t => t.CreatedDate).IsRequired();
            builder.Property(t => t.Scope).HasMaxLength(512);
            builder.Property(t => t.ResponseType).HasMaxLength(512);
            builder.Property(t => t.IconUrl).HasMaxLength(512);
            builder.Property(t => t.PostLogoutUri).HasMaxLength(512);
            builder.Property(t => t.LoginServiceType);

            // Table & Column Mappings
            builder.ToTable("LoginService", "app");
            builder.Property(t => t.LoginServiceID).HasColumnName("LoginServiceID");
            builder.Property(t => t.SiteID).HasColumnName("SiteID");
            builder.Property(t => t.Authority).HasColumnName("Authority");
            builder.Property(t => t.ClientId).HasColumnName("ClientId");
            builder.Property(t => t.ClientSecret).HasColumnName("ClientSecret");
            builder.Property(t => t.MetadataAddress).HasColumnName("MetadataAddress");
            builder.Property(t => t.IconUrl).HasColumnName("IconUrl");
            builder.Property(t => t.PrimaryClaimType).HasColumnName("PrimaryClaimType");
            builder.Property(t => t.SecondaryClaimType).HasColumnName("SecondaryClaimType");
            builder.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            builder.Property(t => t.Disabled).HasColumnName("Disabled");
            builder.Property(t => t.Scope).HasColumnName("Scope");
            builder.Property(t => t.ResponseType).HasColumnName("ResponseType");
            builder.Property(t => t.PostLogoutUri).HasColumnName("PostLogoutUri");
            builder.Property(t => t.RedirectUri).HasColumnName("RedirectUri");
            builder.Property(t => t.LoginServiceType).HasColumnName("LoginServiceType");
            // Relationships

        }
	}
}

