using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CoursePlanningCycleConfiguration : BaseEntityTypeConfiguration<CAM_CoursePlanningCycle>
    {
        public override void Configure(EntityTypeBuilder<CAM_CoursePlanningCycle> builder)
        {
            builder.HasKey(e => e.CoursePlanningCycleId);

            builder.ToTable("cam_CoursePlanningCycle", "staging");

            builder.Property(e => e.CoursePlanningCycleId).ValueGeneratedNever();

            builder.Property(e => e.ChangedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);
        }
    }
}
