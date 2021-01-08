using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_SessionConfiguration : BaseEntityTypeConfiguration<CAM_Session>
    {
        public override void Configure(EntityTypeBuilder<CAM_Session> builder)
        {
            builder.HasKey(e => e.SessionId);

            builder.ToTable("cam_Session", "staging");

            builder.Property(e => e.SessionId).ValueGeneratedNever();

            builder.Property(e => e.ChangedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.SessionCode).HasMaxLength(25);

            builder.HasOne(d => d.ClassRun)
                .WithMany(p => p.CamSession)
                .HasForeignKey(d => d.ClassRunId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
