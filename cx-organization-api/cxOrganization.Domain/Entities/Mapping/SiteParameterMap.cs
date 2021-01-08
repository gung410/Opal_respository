using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class SiteParameterMap.
    /// </summary>
	public class SiteParameterMap : IEntityTypeConfiguration<SiteParameterEntity>
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteParameterMap"/> class.
        /// </summary>
		public void Configure(EntityTypeBuilder<SiteParameterEntity> builder)
        {
			// Primary Key
			builder.HasKey(t => t.SiteParameterId);

			// Properties
			builder.Property(t => t.Key)
				.IsRequired()
				.HasMaxLength(256);
				
			builder.Property(t => t.Value)
				.IsRequired()
				.HasMaxLength(256);
				
			// Table & Column Mappings
            builder.ToTable("SiteParameter", "app");
			builder.Property(t => t.SiteParameterId).HasColumnName("SiteParameterID");
			builder.Property(t => t.SiteId).HasColumnName("SiteID");
			builder.Property(t => t.Key).HasColumnName("Key");
			builder.Property(t => t.Value).HasColumnName("Value");
			builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.GroupName ).HasColumnName("GroupName");
		}
	}
}

