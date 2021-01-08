using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserTrackConfiguration : BaseEntityTypeConfiguration<SAM_UserTrack>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserTrack> builder)
        {
            builder.HasKey(e => new { e.UserHistoryId, e.TrackId });

            builder.ToTable("sam_User_Track", "staging");

            builder.Property(e => e.TrackId).HasColumnName("TrackID");

            builder.HasOne(d => d.Track)
                .WithMany(p => p.SamUserTrack)
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.SamUserTrack)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
