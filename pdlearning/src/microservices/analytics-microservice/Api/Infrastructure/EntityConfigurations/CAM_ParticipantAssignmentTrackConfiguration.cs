using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_ParticipantAssignmentTrackConfiguration : BaseEntityTypeConfiguration<CAM_ParticipantAssignmentTrack>
    {
        public override void Configure(EntityTypeBuilder<CAM_ParticipantAssignmentTrack> builder)
        {
            builder.HasKey(e => e.ParticipantAssignmentTrackId);

            builder.ToTable("cam_ParticipantAssignmentTrack", "staging");

            builder.Property(e => e.ParticipantAssignmentTrackId).ValueGeneratedNever();

            builder.Property(e => e.ChangedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(30);
        }
    }
}
