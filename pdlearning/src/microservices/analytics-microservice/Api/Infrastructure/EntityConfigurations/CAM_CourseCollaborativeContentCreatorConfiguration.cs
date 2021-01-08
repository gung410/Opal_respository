using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseCollaborativeContentCreatorConfiguration : BaseEntityTypeConfiguration<CAM_CourseCollaborativeContentCreator>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseCollaborativeContentCreator> builder)
        {
            builder.HasKey(e => new { e.CourseId, e.UserId });

            builder.ToTable("cam_Course_CollaborativeContentCreator", "staging");

            builder.Property(e => e.UserId).HasColumnName("userId");

            builder.HasOne(d => d.Course)
                .WithMany(p => p.CamCourseCollaborativeContentCreator)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
