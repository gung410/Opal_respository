using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseConfiguration : BaseEntityTypeConfiguration<CAM_Course>
    {
        public override void Configure(EntityTypeBuilder<CAM_Course> builder)
        {
            builder.HasKey(e => e.CourseId);

            builder.ToTable("cam_Course", "staging");

            builder.Property(e => e.CourseId)
                .HasColumnName("CourseID")
                .ValueGeneratedNever();

            builder.Property(e => e.AcknowledgementAndCredit).HasMaxLength(4000);

            builder.Property(e => e.AllowNonCommerInMoereuseWithModification).HasColumnName("AllowNonCommerInMOEReuseWithModification");

            builder.Property(e => e.ChangedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.ContentStatus)
                .HasMaxLength(30)
                .IsFixedLength();

            builder.Property(e => e.CourseCode).HasMaxLength(64);

            builder.Property(e => e.CourseFee).HasColumnType("decimal(18, 2)");

            builder.Property(e => e.CourseLevelId).HasColumnName("CourseLevelID");

            builder.Property(e => e.CourseName).HasMaxLength(2000);

            builder.Property(e => e.CourseType).HasMaxLength(15);

            builder.Property(e => e.CreatedByDepartmentId).HasMaxLength(64);

            builder.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.EcertificatePrerequisite)
                .HasColumnName("ECertificatePrerequisite")
                .HasMaxLength(50);

            builder.Property(e => e.EcertificateTemplateId).HasColumnName("ECertificateTemplateId");

            builder.Property(e => e.ExternalCode).HasMaxLength(50);

            builder.Property(e => e.FullTextSearch).HasMaxLength(2100);

            builder.Property(e => e.FullTextSearchKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.JobFamilyId).HasColumnName("JobFamilyID");

            builder.Property(e => e.LearningModeId).HasColumnName("LearningModeID");

            builder.Property(e => e.MoeofficerId).HasColumnName("MOEOfficerId");

            builder.Property(e => e.NotionalCost).HasColumnType("decimal(18, 2)");

            builder.Property(e => e.PdactivityTypeId).HasColumnName("PDActivityTypeID");

            builder.Property(e => e.PdareaThemeCode)
                .HasColumnName("PDAreaThemeCode")
                .HasMaxLength(50);

            builder.Property(e => e.PdareaThemeId).HasColumnName("PDAreaThemeId");

            builder.Property(e => e.PlaceOfWork).HasMaxLength(50);

            builder.Property(e => e.RegistrationMethod).HasMaxLength(50);

            builder.Property(e => e.Remarks).HasMaxLength(4000);

            builder.Property(e => e.Source).HasMaxLength(255);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("(N'Draft')");

            builder.Property(e => e.VerifiedDate).HasColumnType("datetime");

            builder.HasOne(d => d.CourseLevel)
                .WithMany(p => p.CamCourse)
                .HasForeignKey(d => d.CourseLevelId);

            builder.HasOne(d => d.CoursePlanningCycle)
                .WithMany(p => p.CamCourse)
                .HasForeignKey(d => d.CoursePlanningCycleId);

            builder.HasOne(d => d.EcertificateTemplate)
                .WithMany(p => p.CamCourse)
                .HasForeignKey(d => d.EcertificateTemplateId);

            builder.HasOne(d => d.JobFamily)
                .WithMany(p => p.CamCourse)
                .HasForeignKey(d => d.JobFamilyId);

            builder.HasOne(d => d.LearningMode)
                .WithMany(p => p.CamCourse)
                .HasForeignKey(d => d.LearningModeId);

            builder.HasOne(d => d.NatureOfCourse)
                .WithMany(p => p.CamCourse)
                .HasForeignKey(d => d.NatureOfCourseId);

            builder.HasOne(d => d.PdactivityType)
                .WithMany(p => p.CamCourse)
                .HasForeignKey(d => d.PdactivityTypeId);

            builder.HasOne(d => d.PostCourseEvaluationForm)
                .WithMany(p => p.CamCoursePostCourseEvaluationForm)
                .HasForeignKey(d => d.PostCourseEvaluationFormId);

            builder.HasOne(d => d.PreCourseEvaluationForm)
                .WithMany(p => p.CamCoursePreCourseEvaluationForm)
                .HasForeignKey(d => d.PreCourseEvaluationFormId);
        }
    }
}
