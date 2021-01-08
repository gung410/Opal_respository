using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseNieAcademicGroupsConfiguration : BaseEntityTypeConfiguration<CAM_CourseNieAcademicGroups>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseNieAcademicGroups> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.Name });

            builder.ToTable("cam_Course_NieAcademicGroups", "staging");

            builder.Property(e => e.Name).HasMaxLength(64);
        }
    }
}
