using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserProfessionalInterestAreaConfiguration : BaseEntityTypeConfiguration<SAM_UserProfessionalInterestArea>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserProfessionalInterestArea> builder)
        {
            builder.HasKey(e => new { e.UserHistoryId, e.SubjectId });

            builder.ToTable("sam_User_ProfessionalInterestArea", "staging");

            builder.Property(e => e.SubjectId).HasColumnName("SubjectID");

            builder.HasOne(d => d.Subject)
                .WithMany(p => p.SamUserProfessionalInterestArea)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.SamUserProfessionalInterestArea)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
