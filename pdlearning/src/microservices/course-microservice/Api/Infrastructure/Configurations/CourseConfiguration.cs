using System;
using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Infrastructure.Configurations
{
    public class CourseConfiguration : BaseConfiguration<CourseEntity>
    {
        public override void Configure(EntityTypeBuilder<CourseEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Course");

            builder.Property(e => e.Version)
                  .HasMaxLength(10)
                   .IsRequired(false)
                  .IsUnicode(false);

            builder.Property(e => e.PDActivityType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

            builder.Property(e => e.LearningMode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

            builder.Property(e => e.CourseCode)
                    .HasMaxLength(EntitiesConstants.CourseCodeLength)
                    .IsUnicode(false);

            builder.Property(e => e.ExternalCode)
                    .HasMaxLength(EntitiesConstants.ExternalCodeLength)
                    .IsUnicode(false);

            builder.Property(e => e.CourseLevel)
                    .IsRequired(false)
                    .HasMaxLength(50)
                    .IsUnicode(false);

            builder.Property(e => e.PDAreaThemeId)
                    .IsRequired(false)
                    .HasMaxLength(50)
                    .IsUnicode(false);

            builder.Property(e => e.CourseName)
                    .IsRequired(false)
                    .HasMaxLength(EntitiesConstants.CourseNameLength);

            builder.Property(e => e.ExternalId)
                    .HasMaxLength(255)
                    .IsUnicode(false);

            builder.Property(e => e.ThumbnailUrl)
                   .HasMaxLength(300)
                   .IsUnicode(false);

            builder.Property(e => e.AcknowledgementAndCredit)
                .IsUnicode()
                .HasMaxLength(4000);

            builder.Property(e => e.Remarks)
               .IsUnicode()
               .HasMaxLength(4000);

            builder.Property(e => e.PDAreaThemeCode)
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .IsRequired(false);

            builder.Property(e => e.Status)
               .HasConversion(new EnumToStringConverter<CourseStatus>())
               .HasDefaultValue(CourseStatus.Draft)
               .HasMaxLength(30)
               .IsUnicode(false);

            builder.Property(e => e.ContentStatus)
               .HasConversion(new EnumToStringConverter<ContentStatus>())
               .HasDefaultValue(ContentStatus.Draft)
               .HasMaxLength(30)
               .IsUnicode(false);

            builder.Property(e => e.CourseType)
               .HasConversion(new EnumToStringConverter<CourseType>())
               .HasMaxLength(15)
               .IsUnicode(false)
               .HasDefaultValue(CourseType.New);

            builder.Property(e => e.ECertificatePrerequisite)
               .HasConversion(new EnumToStringConverter<PrerequisiteCertificateType>())
               .HasMaxLength(50)
               .IsUnicode(false);

            builder.Property(e => e.PlaceOfWork)
               .HasConversion(new EnumToStringConverter<PlaceOfWorkType>())
               .HasMaxLength(50)
               .HasDefaultValue(PlaceOfWorkType.ApplicableForEveryone);

            builder.Property(e => e.RegistrationMethod)
               .HasConversion(new EnumToStringConverter<RegistrationMethod>())
               .HasMaxLength(50)
               .IsRequired(false);

            builder.Property(e => e.OwnerDivisionIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<int>>(v, null) : null);

            builder.Property(e => e.OwnerBranchIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<int>>(v, null) : null);

            builder.Property(e => e.PartnerOrganisationIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<int>>(v, null) : null);

            builder.Property(e => e.CategoryIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TrainingAgency)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.OtherTrainingAgencyReason)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.PrerequisiteCourseIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<Guid>>(v, null) : null);

            builder.Property(e => e.ApplicableDivisionIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<int>>(v, null) : null);

            builder.Property(e => e.ApplicableBranchIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<int>>(v, null) : null);

            builder.Property(e => e.ApplicableZoneIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<int>>(v, null) : null);

            builder.Property(e => e.ApplicableClusterIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<int>>(v, null) : null);

            builder.Property(e => e.ApplicableSchoolIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<int>>(v, null) : null);

            builder.Property(e => e.EasSubstantiveGradeBandingIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TrackIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.DevelopmentalRoleIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TeachingLevels)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TeachingSubjectIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TeachingCourseStudyIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.CocurricularActivityIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.JobFamily)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.ServiceSchemeIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.SubjectAreaIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.LearningFrameworkIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.LearningDimensionIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.LearningAreaIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.LearningSubAreaIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.TeacherOutcomeIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.PdActivityPeriods)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.MetadataKeys)
                .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            builder.Property(e => e.CollaborativeContentCreatorIds)
               .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<Guid>>(v, null) : null);

            builder.Property(e => e.CourseFacilitatorIds)
                .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<Guid>>(v, null) : null);

            builder.Property(e => e.CourseCoFacilitatorIds)
                .HasConversion(
                        v => v != null ? JsonSerializer.Serialize(v, null) : null,
                        v => v != null ? JsonSerializer.Deserialize<IEnumerable<Guid>>(v, null) : null);

            builder.Property(e => e.NieAcademicGroups)
                .HasConversion(
                    v => v != null ? JsonSerializer.Serialize(v, null) : null,
                    v => v != null ? JsonSerializer.Deserialize<IEnumerable<string>>(v, null) : null);

            // Technical Columns
            builder.Property(e => e.FullTextSearch)
                    .IsRequired(false);
            builder.Property(e => e.FullTextSearchKey)
                    .IsRequired(true)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValue(string.Empty);

            // Course Planning Information
            builder.Property(e => e.NatureOfCourse)
              .HasMaxLength(50)
              .IsUnicode(false)
              .IsRequired(false);

            builder.Property(e => e.WillArchiveCommunity)
                .HasDefaultValue(true);

            builder.HasMany(p => p.CourseInternalValues)
                .WithOne(p => p.Course)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => new { p.DepartmentId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.ECertificateTemplateId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.FirstAdministratorId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.SecondAdministratorId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.MOEOfficerId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.PDAreaThemeId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.PrimaryApprovingOfficerId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.AlternativeApprovingOfficerId, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.CourseCode, p.IsDeleted, p.FullTextSearchKey });

            // Because RDS Devop problem, maximum indexing is 1700 byte
            // builder.HasIndex(p => p.CourseName);
            builder.HasIndex(p => new { p.ExternalCode, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.Status, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.ContentStatus, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.CreatedBy, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.IsDeleted, p.ChangedDate });
            builder.HasIndex(p => new { p.IsDeleted, p.ArchiveDate });
            builder.HasIndex(p => new { p.CreatedBy, p.Status, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.FirstAdministratorId, p.Status, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.SecondAdministratorId, p.Status, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.PrimaryApprovingOfficerId, p.Status, p.IsDeleted, p.FullTextSearchKey });
            builder.HasIndex(p => new { p.AlternativeApprovingOfficerId, p.Status, p.IsDeleted, p.FullTextSearchKey });

            // Technical Columns Indexes
            builder.HasIndex(p => p.FullTextSearchKey).IsUnique();
        }
    }
}
