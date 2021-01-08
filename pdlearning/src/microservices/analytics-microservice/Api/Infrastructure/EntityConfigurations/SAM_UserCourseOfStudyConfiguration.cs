using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserCourseOfStudyConfiguration : BaseEntityTypeConfiguration<SAM_UserCourseOfStudy>
    {
        public override void Configure(EntityTypeBuilder<SAM_UserCourseOfStudy> builder)
        {
            builder.HasKey(e => new { e.UserHistoryId, e.CourseOfStudyId });

            builder.ToTable("sam_User_CourseOfStudy", "staging");

            builder.Property(e => e.CourseOfStudyId).HasColumnName("CourseOfStudyID");

            builder.HasOne(d => d.CourseOfStudy)
                .WithMany(p => p.SamUserCourseOfStudy)
                .HasForeignKey(d => d.CourseOfStudyId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserHistory)
                .WithMany(p => p.SamUserCourseOfStudy)
                .HasForeignKey(d => d.UserHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
