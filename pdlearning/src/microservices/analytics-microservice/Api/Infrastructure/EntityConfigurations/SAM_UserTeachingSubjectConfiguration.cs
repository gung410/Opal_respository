using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserTeachingSubjectConfiguration : BaseEntityTypeConfiguration<SAM_UserTeachingSubject>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserTeachingSubject> builder)
        {
            builder.HasKey(e => new { e.UserHistoryId, e.TeachingSubjectId });

            builder.ToTable("sam_User_TeachingSubject", "staging");

            builder.Property(e => e.TeachingSubjectId).HasColumnName("TeachingSubjectID");

            builder.HasOne(d => d.TeachingSubject)
                .WithMany(p => p.SamUserTeachingSubject)
                .HasForeignKey(d => d.TeachingSubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.SamUserTeachingSubject)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
