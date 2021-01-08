using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class OwnerMap.
    /// </summary>
    public class OwnerMap : IEntityTypeConfiguration<OwnerEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<OwnerEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.OwnerId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.ReportServer)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.ReportDB)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.OLAPServer)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.OLAPDB)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.Css)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.Url)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.Prefix)
                .IsRequired()
                .HasMaxLength(8);

            builder.Property(t => t.Description)
                .IsRequired();

            builder.Property(t => t.UseHashPassword)
               .IsRequired();

            builder.Property(t => t.UseOTP)
                .IsRequired();

            builder.Property(t => t.DefaultHashMethod)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("Owner", "org");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ReportServer).HasColumnName("ReportServer");
            builder.Property(t => t.ReportDB).HasColumnName("ReportDB");
            builder.Property(t => t.MainHierarchyId).HasColumnName("MainHierarchyID");
            builder.Property(t => t.OLAPServer).HasColumnName("OLAPServer");
            builder.Property(t => t.OLAPDB).HasColumnName("OLAPDB");
            builder.Property(t => t.Css).HasColumnName("Css");
            builder.Property(t => t.Url).HasColumnName("Url");
            builder.Property(t => t.LoginType).HasColumnName("LoginType");
            builder.Property(t => t.Prefix).HasColumnName("Prefix");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.Logging).HasColumnName("Logging");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.OTPLength).HasColumnName("OTPLength");
            builder.Property(t => t.OTPCharacters).HasColumnName("OTPCharacters");
            builder.Property(t => t.OTPAllowLowercase).HasColumnName("OTPAllowLowercase");
            builder.Property(t => t.OTPAllowUppercase).HasColumnName("OTPAllowUppercase");
            builder.Property(t => t.UseOTPCaseSensitive).HasColumnName("UseOTPCaseSensitive");
            builder.Property(t => t.UseHashPassword).HasColumnName("UseHashPassword");
            builder.Property(t => t.UseOTP).HasColumnName("UseOTP");
            builder.Property(t => t.OTPDuration).HasColumnName("OTPDuration");
            builder.Property(t => t.DefaultHashMethod).HasColumnName("DefaultHashMethod");
        }
    }
}