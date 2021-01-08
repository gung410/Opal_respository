using Microservice.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Analytics.Infrastructure.EntityConfigurations
{
    public class CAM_CourseHistoryConfiguration : BaseEntityTypeConfiguration<CAM_CourseHistory>
    {
        public override void Configure(EntityTypeBuilder<CAM_CourseHistory> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("cam_Course_History", "staging");

            builder.Property(e => e.CourseId).HasColumnName("CourseID");

            builder.Property(e => e.No).HasColumnName("VersionNo");

            builder.Property(e => e.AcknowledgementAndCredit).HasMaxLength(4000);

            builder.Property(e => e.AllowNonCommerInMoereuseWithModification).HasColumnName("AllowNonCommerInMOEReuseWithModification");

            builder.Property(e => e.ContentStatus)
                .HasMaxLength(30);

            builder.Property(e => e.CourseCode).HasMaxLength(64);

            builder.Property(e => e.CourseFee).HasColumnType("decimal(18, 2)");

            builder.Property(e => e.CourseLevelId).HasColumnName("CourseLevelID");

            builder.Property(e => e.CourseName).HasMaxLength(2000);

            builder.Property(e => e.CourseType).HasMaxLength(15);

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
                .IsUnicode(false);

            builder.Property(e => e.VerifiedDate).HasColumnType("datetime");
        }
    }
}
