using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    public class LtMemberRoleMap : IEntityTypeConfiguration<LtMemberRoleEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtUserTypeMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LtMemberRoleEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.LanguageId, t.MemberRoleId });

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("LT_MemberRole", "org");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.MemberRoleId).HasColumnName("MemberRoleID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");

            builder.HasOne(t => t.MemberRole)
                .WithMany(t => t.LT_MemberRoles)
                .HasForeignKey(d => d.MemberRoleId);
        }
    }
}
