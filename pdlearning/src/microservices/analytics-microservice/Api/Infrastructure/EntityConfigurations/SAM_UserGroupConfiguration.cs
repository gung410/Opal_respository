using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserGroupConfiguration : BaseEntityTypeConfiguration<SAM_UserGroup>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserGroup> builder)
        {
            builder.HasKey(e => e.UserGroupId);

            builder.ToTable("sam_UserGroup", "staging");

            builder.Property(e => e.UserGroupId)
                .HasColumnName("UserGroupID")
                .ValueGeneratedNever();

            builder.Property(e => e.ArchetypeId)
                .HasColumnName("ArchetypeID")
                .HasMaxLength(64);

            builder.Property(e => e.DepartmentId)
                .HasColumnName("DepartmentID")
                .HasMaxLength(64);

            builder.Property(e => e.Description).IsRequired();

            builder.Property(e => e.EntityStatusId).HasColumnName("EntityStatusID");

            builder.Property(e => e.EntityStatusReasonId).HasColumnName("EntityStatusReasonID");

            builder.Property(e => e.EntityVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();

            builder.Property(e => e.ExtId)
                .IsRequired()
                .HasColumnName("ExtID")
                .HasMaxLength(256);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.Tag).IsRequired();

            builder.Property(e => e.UserGroupTypeId).HasColumnName("UserGroupTypeID");

            builder.Property(e => e.UserId).HasColumnName("UserID");
        }
    }
}
