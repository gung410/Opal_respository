using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserJobFamilyConfiguration : BaseEntityTypeConfiguration<SAM_UserJobFamily>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserJobFamily> builder)
        {
            builder.HasKey(e => new { e.UserHistoryId, e.JobFamilyId });

            builder.ToTable("sam_User_JobFamily", "staging");

            builder.Property(e => e.JobFamilyId).HasColumnName("JobFamilyID");

            builder.HasOne(d => d.JobFamily)
                .WithMany(p => p.SamUserJobFamily)
                .HasForeignKey(d => d.JobFamilyId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.SamUserJobFamily)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
