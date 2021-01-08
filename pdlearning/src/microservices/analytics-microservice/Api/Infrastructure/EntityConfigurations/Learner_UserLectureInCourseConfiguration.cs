using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserLectureInCourseConfiguration : BaseEntityTypeConfiguration<Learner_UserLectureInCourse>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserLectureInCourse> builder)
        {
            builder.HasKey(e => e.UserLectureInCourseId);

            builder.ToTable("learner_UserLectureInCourse", "staging");

            builder.Property(e => e.UserLectureInCourseId).ValueGeneratedNever();

            builder.Property(e => e.ReviewStatus).HasMaxLength(1000);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);

            builder.Property(e => e.Version)
                .HasMaxLength(5)
                .IsUnicode(false);

            builder.HasOne(d => d.Lecture)
                .WithMany(p => p.LearnerUserLectureInCourse)
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
