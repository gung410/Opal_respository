using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserOtpClaimConfiguration : BaseEntityTypeConfiguration<SAM_UserOtpClaim>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserOtpClaim> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("ClaimId");

            builder.ToTable("sam_User_OTPClaims", "staging");

            builder.Property(e => e.Id)
                .HasColumnName("ClaimID")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Departmentid).HasMaxLength(64);

            builder.Property(e => e.SessionId).HasColumnName("SessionID");

            builder.Property(e => e.Type).HasMaxLength(64);

            builder.Property(e => e.UserId).HasColumnName("UserID");

            builder.HasOne(d => d.Department)
                .WithMany(p => p.SamUserOtpClaims)
                .HasForeignKey(d => d.Departmentid);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.SamUserOtpClaims)
                .HasForeignKey(d => d.UserHistoryId);
        }
    }
}
