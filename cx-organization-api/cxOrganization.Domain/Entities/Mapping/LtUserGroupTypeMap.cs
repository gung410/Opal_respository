using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LT_UserGroupTypeMap.
    /// </summary>
    public class LtUserGroupTypeMap : IEntityTypeConfiguration<LtUserGroupTypeEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtUserGroupTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtUserGroupTypeEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.LanguageId, t.UserGroupTypeId });

            // Properties

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_UserGroupType","org");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.UserGroupTypeId).HasColumnName("UserGroupTypeID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            //builder.HasOne(t => t.Language)
            //    .WithMany(t => t.LT_UserGroupType)
            //    .HasForeignKey(d => d.LanguageId);

            builder.HasOne(t => t.UserGroupType)
                .WithMany(t => t.LT_UserGroupType)
                .HasForeignKey(d => d.UserGroupTypeId);

        }
    }
}
