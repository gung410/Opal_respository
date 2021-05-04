using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class UserMap.
    /// </summary>
    public class UserMap : IEntityTypeConfiguration<UserEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.UserId);

            // Properties
            builder.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Mobile)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.SSN)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.SSNHash)
            .HasMaxLength(64);

            builder.Property(t => t.Tag)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(t => t.HashPassword)
               .IsRequired()
               .HasMaxLength(128);

            builder.Property(t => t.SaltPassword)
               .IsRequired()
               .HasMaxLength(64);

            builder.Property(t => t.OneTimePassword)
               .IsRequired()
               .HasMaxLength(64);

            builder.Property(t => t.CountryCode);

            builder.Property(p => p.EntityVersion).IsConcurrencyToken();



            // Table & Column Mappings
            builder.ToTable("User", "org");
            builder.Property(t => t.UserId).HasColumnName("UserID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.DepartmentId).HasColumnName("DepartmentID");
            builder.Property(t => t.LanguageId).HasColumnName("LanguageID");
            builder.Property(t => t.RoleId).HasColumnName("RoleID");
            builder.Property(t => t.UserName).HasColumnName("UserName");
            builder.Property(t => t.Password).HasColumnName("Password");
            builder.Property(t => t.LastName).HasColumnName("LastName");
            builder.Property(t => t.FirstName).HasColumnName("FirstName");
            builder.Property(t => t.Email).HasColumnName("Email");
            builder.Property(t => t.Mobile).HasColumnName("Mobile");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
            builder.Property(t => t.SSN).HasColumnName("SSN");
            builder.Property(t => t.SSNHash).HasColumnName("SSNHash");
            builder.Property(t => t.Tag).HasColumnName("Tag");
            builder.Property(t => t.Locked).HasColumnName("Locked");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.ChangePassword).HasColumnName("ChangePassword");
            builder.Property(t => t.Gender).HasColumnName("Gender");
            builder.Property(t => t.DateOfBirth).HasColumnName("DateOfBirth");
            builder.Property(t => t.HashPassword).HasColumnName("HashPassword");
            builder.Property(t => t.SaltPassword).HasColumnName("SaltPassword");
            builder.Property(t => t.OneTimePassword).HasColumnName("OneTimePassword");
            builder.Property(t => t.OTPExpireTime).HasColumnName("OTPExpireTime");
            builder.Property(t => t.CountryCode).HasColumnName("CountryCode");
            builder.Property(t => t.ForceUserLoginAgain).HasColumnName("ForceUserLoginAgain");
            builder.Property(t => t.EntityVersion).HasColumnName("EntityVersion");
            builder.Property(t => t.LastUpdated).HasColumnName("LastUpdated");
            builder.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            builder.Property(t => t.LastSynchronized).HasColumnName("LastSynchronized");
            builder.Property(t => t.ArchetypeId).HasColumnName("ArchetypeID");
            builder.Property(t => t.Deleted).HasColumnName("Deleted");
            builder.Property(t => t.EntityExpirationDate).HasColumnName("EntityExpirationDate");
            builder.Property(t => t.EntityStatusId).HasColumnName("EntityStatusID");
            builder.Property(t => t.EntityStatusReasonId).HasColumnName("EntityStatusReasonID");
            builder.Property(t => t.CustomerId).HasColumnName("CustomerID");
            builder.Property(t => t.DynamicAttributes).HasColumnName("DynamicAttributes");

            builder.HasOne(t => t.Department)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.DepartmentId);

            builder.HasMany(t => t.UGMembers)
                .WithOne(ugu => ugu.User)
                .HasForeignKey(ugu => ugu.UserId);

            builder.HasMany(t => t.LoginServiceUsers)
                .WithOne(ugu => ugu.User)
                .HasForeignKey(ugu => ugu.UserId);
        }
    }
}