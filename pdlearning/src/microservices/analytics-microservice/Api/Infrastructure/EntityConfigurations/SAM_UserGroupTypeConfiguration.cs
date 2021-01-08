using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserGroupTypeConfiguration : BaseEntityTypeConfiguration<SAM_UserGroupType>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserGroupType> builder)
        {
            builder.HasKey(e => e.UserGroupTypeId);

            builder.ToTable("sam_UserGroupType", "staging");

            builder.Property(e => e.UserGroupTypeId)
                .HasColumnName("UserGroupTypeID")
                .ValueGeneratedNever();

            builder.Property(e => e.Created).HasColumnType("smalldatetime");

            builder.Property(e => e.ExtId)
                .IsRequired()
                .HasColumnName("ExtID")
                .HasMaxLength(256);

            builder.Property(e => e.MasterId).HasColumnName("MasterID");

            builder.Property(e => e.Name).HasMaxLength(256);
        }
    }
}
