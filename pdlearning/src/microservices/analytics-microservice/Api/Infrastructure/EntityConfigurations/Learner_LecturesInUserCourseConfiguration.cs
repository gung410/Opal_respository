using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_LecturesInUserCourseConfiguration : BaseEntityTypeConfiguration<Learner_LecturesInUserCourse>
    {
        public override void Configure(EntityTypeBuilder<Learner_LecturesInUserCourse> builder)
        {
            builder.HasKey(e => e.LecturesInUserCourseId);

            builder.ToTable("learner_LecturesInUserCourse", "staging");

            builder.Property(e => e.LecturesInUserCourseId).ValueGeneratedNever();

            builder.Property(e => e.ReviewStatus).HasMaxLength(1000);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);

            builder.Property(e => e.Version)
                .HasMaxLength(5)
                .IsUnicode(false);

            builder.HasOne(d => d.Lecture)
                .WithMany(p => p.LearnerLecturesInUserCourse)
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(d => d.UserCourse)
                .WithMany(p => p.LearnerLecturesInUserCourse)
                .HasForeignKey(d => d.UserCourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
