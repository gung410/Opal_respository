using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// User group user mapping
    /// </summary>
    public class UGMemberMap : IEntityTypeConfiguration<UGMemberEntity>
    {
        public void Configure(EntityTypeBuilder<UGMemberEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.UGMemberId);

            // Table & Column Mappings
            builder.ToTable("UGMember", "org");
            builder.Property(t => t.UGMemberId).HasColumnName("UGMemberID");
            builder.Property(t => t.UserGroupId).HasColumnName("UserGroupID");
            builder.Property(t => t.UserId).HasColumnName("UserID");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            builder.Property(t => t.MemberRoleId).HasColumnName("MemberRoleID");
            builder.Property(t => t.validFrom).HasColumnName("validFrom");

            builder.Property(t => t.ValidTo).HasColumnName("ValidTo");
            builder.Property(t => t.EntityVersion).HasColumnName("EntityVersion");
            builder.Property(t => t.LastUpdated).HasColumnName("LastUpdated");
            builder.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            builder.Property(t => t.LastSynchronized).HasColumnName("LastSynchronized");
            //builder.Property(t => t.Deleted).HasColumnName("Deleted");

            builder.Property(t => t.EntityStatusId).HasColumnName("EntityStatusID");
            builder.Property(t => t.EntityStatusReasonId).HasColumnName("EntityStatusReasonID");
            builder.Property(t => t.CustomerId).HasColumnName("CustomerID");
            builder.Property(t => t.PeriodId).HasColumnName("PeriodID");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
            builder.Property(t => t.DisplayName).HasColumnName("DisplayName");
            builder.Property(t => t.ReferrerResource).HasColumnName("ReferrerResource");
            builder.Property(t => t.ReferrerArchetypeId).HasColumnName("ReferrerArchetypeID");
            builder.Property(t => t.ReferrerToken).HasColumnName("ReferrerToken");

            builder.HasOne(t => t.MemberRole)
                .WithMany(t => t.UGMembers)
                .HasForeignKey(d => d.MemberRoleId);
        }
    }
}
