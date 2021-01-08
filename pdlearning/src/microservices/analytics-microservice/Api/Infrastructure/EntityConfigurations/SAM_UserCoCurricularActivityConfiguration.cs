using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserCoCurricularActivityConfiguration : BaseEntityTypeConfiguration<SAM_UserCoCurricularActivity>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserCoCurricularActivity> builder)
        {
            builder.HasKey(e => new { e.UserHistoryId, e.CoCurricularActivityId });

            builder.ToTable("sam_User_CoCurricularActivity", "staging");

            builder.Property(e => e.CoCurricularActivityId).HasColumnName("CoCurricularActivityID");

            builder.HasOne(d => d.CoCurricularActivity)
                .WithMany(p => p.SamUserCoCurricularActivity)
                .HasForeignKey(d => d.CoCurricularActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.SamUserCoCurricularActivity)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
