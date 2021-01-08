using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LtUserTypeMap.
    /// </summary>
    public class LtUserTypeMap : IEntityTypeConfiguration<LtUserTypeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtUserTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtUserTypeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new {t.LanguageId, t.UserTypeId});

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_UserType", "org");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.UserTypeId).HasColumnName("UserTypeID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");

            builder.HasOne(t => t.UserType)
                .WithMany(t => t.LT_UserType)
                .HasForeignKey(d => d.UserTypeId);
        }
    }
}