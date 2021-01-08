using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class LoginService_UserMap.
    /// </summary>
    public class LoginServiceUserMap : IEntityTypeConfiguration<LoginServiceUserEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginServiceUserMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<LoginServiceUserEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.LoginServiceId, t.UserId });

            builder.Property(t => t.PrimaryClaimValue)
                .IsRequired()
                .HasMaxLength(512);

            // Table & Column Mappings
            builder.ToTable("LoginService_User","app");
            builder.Property(t => t.LoginServiceId).HasColumnName("LoginServiceID");
            builder.Property(t => t.UserId).HasColumnName("UserID");
            builder.Property(t => t.PrimaryClaimValue).HasColumnName("PrimaryClaimValue");
            builder.Property(t => t.Created).HasColumnName("Created");

        }
    }
}
