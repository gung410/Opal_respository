using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserUserTypesConfiguration : BaseEntityTypeConfiguration<SAM_UserUserTypes>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserUserTypes> builder)
        {
            builder.HasKey(e => new { e.UserHistoryId, e.UserTypeId });

            builder.ToTable("sam_User_UserTypes", "staging");

            builder.Property(e => e.UserTypeId).HasMaxLength(64);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.SamUserUserTypes)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserType)
                .WithMany(p => p.SamUserUserTypes)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
