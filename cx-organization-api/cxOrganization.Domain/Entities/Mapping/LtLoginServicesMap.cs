using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LT_LoginServicesMap.
    /// </summary>
	public class LtLoginServicesMap : IEntityTypeConfiguration<LtLoginService>
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="LtLoginServicesMap"/> class.
        /// </summary>
		public void Configure(EntityTypeBuilder<LtLoginService> builder)
        {
			// Primary Key
			builder.HasKey(t => new { t.LanguageId, t.LoginServiceId });
				
			builder.Property(t => t.Name)
				.HasMaxLength(256);
				
			builder.Property(t => t.ToolTip)
				.HasMaxLength(512);
				
			// Table & Column Mappings
            builder.ToTable("LT_LoginService", "app");
			builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
			builder.Property(t => t.LoginServiceId).HasColumnName("LoginServiceID");
			builder.Property(t => t.Name).HasColumnName("Name");
			builder.Property(t => t.Description).HasColumnName("Description");
			builder.Property(t => t.ToolTip).HasColumnName("ToolTip");

			// Relationships
			builder.HasOne(t => t.LoginServiceEntity)
				.WithMany(t => t.LT_LoginServices)
				.HasForeignKey(d => d.LoginServiceId);
				
            //builder.HasOne(t => t.Language)
            //    .WithMany(t => t.LT_LoginServices)
            //    .HasForeignKey(d => d.LanguageId);
				
		}
	}
}

