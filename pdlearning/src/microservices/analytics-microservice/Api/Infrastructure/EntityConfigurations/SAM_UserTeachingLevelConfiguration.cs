using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserTeachingLevelConfiguration : BaseEntityTypeConfiguration<SAM_UserTeachingLevel>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserTeachingLevel> builder)
        {
            builder.HasKey(e => new { e.UserHistoryId, e.TeachingLevelId });

            builder.ToTable("sam_User_TeachingLevel", "staging");

            builder.Property(e => e.TeachingLevelId).HasColumnName("TeachingLevelID");

            builder.HasOne(d => d.TeachingLevel)
                .WithMany(p => p.SamUserTeachingLevel)
                .HasForeignKey(d => d.TeachingLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.SamUserTeachingLevel)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
