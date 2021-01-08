using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class Learner_UserLearningPackageConfiguration : BaseEntityTypeConfiguration<Learner_UserLearningPackage>
    {
        public override void Configure(EntityTypeBuilder<Learner_UserLearningPackage> builder)
        {
            builder.HasKey(e => e.UserLearningPackageId)
                    .HasName("PK_UserLearningPackage");

            builder.ToTable("learner_UserLearningPackages", "staging");

            builder.Property(e => e.UserLearningPackageId).ValueGeneratedNever();

            builder.Property(e => e.CompletionStatus).HasMaxLength(64);

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.LessonStatus).HasMaxLength(64);

            builder.Property(e => e.SuccessStatus).HasMaxLength(64);

            builder.Property(e => e.Type)
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false);

            builder.HasOne(d => d.UserDigitalContent)
                        .WithMany(p => p.LearnerUserLearningPackages)
                        .HasForeignKey(d => d.UserDigitalContentId);

            builder.HasOne(d => d.UserLectureInCourse)
                        .WithMany(p => p.LearnerUserLearningPackages)
                        .HasForeignKey(d => d.UserLectureInCourseId);

            builder.HasOne(d => d.UserLectureInCourseNavigation)
                        .WithMany(p => p.LearnerUserLearningPackages)
                        .HasForeignKey(d => d.UserLectureInCourseId);
        }
    }
}
