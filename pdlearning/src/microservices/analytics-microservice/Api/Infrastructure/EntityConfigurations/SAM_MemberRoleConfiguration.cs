using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_MemberRoleConfiguration : BaseEntityTypeConfiguration<SAM_MemberRole>
    {
        public override void Configure(EntityTypeBuilder<SAM_MemberRole> builder)
        {
            builder.HasKey(e => e.MemberRoleId);

            builder.ToTable("sam_MemberRole", "staging");

            builder.Property(e => e.MemberRoleId)
                .HasColumnName("MemberRoleID")
                .ValueGeneratedNever();

            builder.Property(e => e.Created).HasColumnType("smalldatetime");

            builder.Property(e => e.EntityStatusId).HasColumnName("EntityStatusID");

            builder.Property(e => e.EntityStatusReasonId).HasColumnName("EntityStatusReasonID");

            builder.Property(e => e.ExtId)
                .HasColumnName("ExtID")
                .HasMaxLength(256);

            builder.Property(e => e.MasterId).HasColumnName("MasterID");

            builder.Property(e => e.Name).HasMaxLength(256);
        }
    }
}
