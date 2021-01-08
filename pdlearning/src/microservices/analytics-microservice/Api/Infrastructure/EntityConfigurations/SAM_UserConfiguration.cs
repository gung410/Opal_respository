using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class SAM_UserConfiguration : BaseEntityTypeConfiguration<SAM_User>
    {
        public override void Configure(EntityTypeBuilder<SAM_User> builder)
        {
            builder.HasKey(e => e.UserId);

            builder.ToTable("sam_User", "staging");

            builder.Property(e => e.UserId).ValueGeneratedNever();

            builder.Property(e => e.CoCircularactivityId).HasColumnName("coCircularactivityID");

            builder.Property(e => e.CourseOfStudyId).HasColumnName("CourseOfStudyID");

            builder.Property(e => e.DepartmentId).HasMaxLength(64);

            builder.Property(e => e.DesignationId)
                .HasColumnName("DesignationID")
                .HasMaxLength(64);

            builder.Property(e => e.Email).HasMaxLength(256);

            builder.Property(e => e.ExtId).HasMaxLength(64);

            builder.Property(e => e.FirstName).HasMaxLength(64);

            builder.Property(e => e.JobFamilyId).HasColumnName("JobFamilyID");

            builder.Property(e => e.JobTitle).HasMaxLength(256);

            builder.Property(e => e.LastName).HasMaxLength(64);

            builder.Property(e => e.ServiceSchemeId).HasColumnName("ServiceSchemeID");

            builder.Property(e => e.SubjectId).HasColumnName("SubjectID");

            builder.Property(e => e.TeachingLevelId).HasColumnName("TeachingLevelID");

            builder.Property(e => e.TeachingSubjectId).HasColumnName("TeachingSubjectID");
        }
    }
}
