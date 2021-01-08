using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseOtherTrainingAgencyReasonConfiguration : BaseEntityTypeConfiguration<CAM_CourseOtherTrainingAgencyReason>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseOtherTrainingAgencyReason> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.Name });

            builder.ToTable("cam_Course_OtherTrainingAgencyReason", "staging");

            builder.Property(e => e.Name).HasMaxLength(255);
        }
    }
}
