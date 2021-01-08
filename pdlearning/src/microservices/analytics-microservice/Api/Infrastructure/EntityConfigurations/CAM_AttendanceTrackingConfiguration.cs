using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_AttendanceTrackingConfiguration : BaseEntityTypeConfiguration<CAM_AttendanceTracking>
    {
        public override void Configure(EntityTypeBuilder<CAM_AttendanceTracking> builder)
        {
            builder.HasKey(e => e.AttendanceTrackingId);

            builder.ToTable("cam_AttendanceTracking", "staging");

            builder.Property(e => e.AttendanceTrackingId).ValueGeneratedNever();

            builder.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.HasOne(d => d.Registration)
                .WithMany(p => p.CamAttendanceTracking)
                .HasForeignKey(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.Session)
                .WithMany(p => p.CamAttendanceTracking)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
