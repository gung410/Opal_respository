using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_ClassRunFacilitatorConfiguration : BaseEntityTypeConfiguration<CAM_ClassRunFacilitator>
    {
        public override void Configure(EntityTypeBuilder<CAM_ClassRunFacilitator> builder)
        {
            builder.HasKey(e => new { e.ClassRunId, e.UserId, e.Type });

            builder.ToTable("cam_ClassRun_Facilitator", "staging");

            builder.Property(e => e.ClassRunId).HasColumnName("ClassRunID");

            builder.Property(e => e.Type).HasMaxLength(32);

            builder.HasOne(d => d.ClassRun)
                .WithMany(p => p.CamClassRunFacilitator)
                .HasForeignKey(d => d.ClassRunId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
