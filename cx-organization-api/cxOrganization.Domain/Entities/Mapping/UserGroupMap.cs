using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cxOrganization.Domain.Entities
{
    /// <summary>
    /// Class UserGroupMap.
    /// </summary>
    public class UserGroupMap : IEntityTypeConfiguration<UserGroupEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupMap"/> class.
        /// </summary>
        public void Configure(EntityTypeBuilder<UserGroupEntity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.UserGroupId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Description)
                .IsRequired();

            builder.Property(t => t.ExtId)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(t => t.Tag)
                .IsRequired();

            builder.Property(p => p.EntityVersion).IsConcurrencyToken();

            // Table & Column Mappings
            builder.ToTable("UserGroup", "org");
            builder.Property(t => t.UserGroupId).HasColumnName("UserGroupID");
            builder.Property(t => t.OwnerId).HasColumnName("OwnerID");
            builder.Property(t => t.DepartmentId).HasColumnName("DepartmentID");
            builder.Property(t => t.SurveyId).HasColumnName("SurveyID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.Created).HasColumnName("Created");
            builder.Property(t => t.ExtId).HasColumnName("ExtID");
            builder.Property(t => t.PeriodId).HasColumnName("PeriodID");
            builder.Property(t => t.UserId).HasColumnName("UserID");
            builder.Property(t => t.Tag).HasColumnName("Tag");
            builder.Property(t => t.EntityVersion).HasColumnName("EntityVersion");
            builder.Property(t => t.LastUpdated).HasColumnName("LastUpdated");
            builder.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            builder.Property(t => t.LastSynchronized).HasColumnName("LastSynchronized");
            builder.Property(t => t.ArchetypeId).HasColumnName("ArchetypeID");
            builder.Property(t => t.Deleted).HasColumnName("Deleted");
            builder.Property(t => t.EntityStatusId).HasColumnName("EntityStatusID");
            builder.Property(t => t.EntityStatusReasonId).HasColumnName("EntityStatusReasonID");
            builder.Property(t => t.ReferrerResource).HasColumnName("ReferrerResource");
            builder.Property(t => t.ReferrerArchetypeId).HasColumnName("ReferrerArchetypeID");
            builder.Property(t => t.ReferrerToken).HasColumnName("ReferrerToken");

            builder.HasOne(t => t.Department)
                .WithMany(t => t.UserGroups)
                .HasForeignKey(d => d.DepartmentId);

            builder.HasOne(t => t.User)
             .WithMany(t => t.UserGroups)
             .HasForeignKey(d => d.UserId);

            builder.HasMany(t => t.UGMembers)
                .WithOne(ugu => ugu.UserGroup)
                .HasForeignKey(ugu => ugu.UserGroupId);
        }
    }
}
