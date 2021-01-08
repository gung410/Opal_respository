using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserHistoryConfiguration : BaseEntityTypeConfiguration<SAM_UserHistory>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserHistory> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("sam_UserHistory", "staging");

            builder.Property(e => e.Id)
                .HasColumnName("UserHistoryId")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.ArcheTypeId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.DesignationId)
                .HasColumnName("DesignationID")
                .HasMaxLength(64);

            builder.Property(e => e.Email).HasMaxLength(256);

            builder.Property(e => e.FirstName).HasMaxLength(64);

            builder.Property(e => e.JobTitle).HasMaxLength(256);

            builder.Property(e => e.LastName).HasMaxLength(64);

            builder.Property(e => e.ExtId).HasMaxLength(256);

            builder.Property(e => e.NotificationPreference)
                .HasColumnName("notificationPreference")
                .HasMaxLength(64);

            builder.Property(e => e.ServiceSchemeId).HasColumnName("ServiceSchemeID");
        }
    }
}
