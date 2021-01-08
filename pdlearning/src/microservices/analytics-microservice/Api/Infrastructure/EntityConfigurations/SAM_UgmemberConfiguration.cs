using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UgmemberConfiguration : BaseEntityTypeConfiguration<SAM_Ugmember>
    {
        public override void Configure(EntityTypeBuilder<SAM_Ugmember> builder)
        {
            builder.HasKey(e => e.UgmemberId);

            builder.ToTable("sam_UGMember", "staging");

            builder.Property(e => e.UgmemberId)
                .HasColumnName("UGMemberID")
                .ValueGeneratedNever();

            builder.Property(e => e.Created).HasColumnType("smalldatetime");

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

            builder.Property(e => e.EntityStatusId).HasColumnName("EntityStatusID");

            builder.Property(e => e.EntityStatusReasonId).HasColumnName("EntityStatusReasonID");

            builder.Property(e => e.EntityVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            builder.Property(e => e.ExtId)
                .IsRequired()
                .HasColumnName("ExtID")
                .HasMaxLength(256);

            builder.Property(e => e.MemberRoleId).HasColumnName("MemberRoleID");

            builder.Property(e => e.UserGroupId).HasColumnName("UserGroupID");

            builder.Property(e => e.UserId).HasColumnName("UserID");
        }
    }
}
