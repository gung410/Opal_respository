﻿// <auto-generated />
using System;
using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Course.Migrations
{
    [DbContext(typeof(CourseDbContext))]
    [Migration("20200415031350_AddFieldVenueForSession")]
    partial class AddFieldVenueForSession
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microservice.Course.Domain.Entities.Assignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ClassRunId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ResourceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Assignment");
                });

            modelBuilder.Entity("Microservice.Course.Domain.Entities.ClassRun", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ApplicationEndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ApplicationStartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ApprovalContentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CancellationStatus")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ClassRunCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ClassRunVenueId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ClassTitle")
                        .HasColumnType("nvarchar(2000)")
                        .HasMaxLength(2000);

                    b.Property<string>("CoFacilitatorIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContentStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasDefaultValue("Draft");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FacilitatorIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("MaxClassSize")
                        .HasColumnType("int");

                    b.Property<int>("MinClassSize")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PublishedContentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegistrationEndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RegistrationStartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RescheduleEndDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RescheduleStartDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RescheduleStatus")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<DateTime?>("StartDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasDefaultValue("Unpublished");

                    b.Property<DateTime?>("SubmittedContentDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ClassRun");
                });

            modelBuilder.Entity("Microservice.Course.Domain.Entities.CourseEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AcknowledgementAndCredit")
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000)
                        .IsUnicode(true);

                    b.Property<bool>("AllowNonCommerInMOEReuseWithModification")
                        .HasColumnType("bit");

                    b.Property<bool>("AllowNonCommerInMoeReuseWithoutModification")
                        .HasColumnType("bit");

                    b.Property<bool>("AllowNonCommerReuseWithModification")
                        .HasColumnType("bit");

                    b.Property<bool>("AllowNonCommerReuseWithoutModification")
                        .HasColumnType("bit");

                    b.Property<bool>("AllowPersonalDownload")
                        .HasColumnType("bit");

                    b.Property<Guid?>("AlternativeApprovingOfficerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApplicableBranchIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApplicableClusterIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApplicableDivisionIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApplicableSchoolIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApplicableZoneIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ApprovalContentDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ArchiveDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CategoryIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CocurricularActivityIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CollaborativeContentCreatorIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContentStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasDefaultValue("Draft");

                    b.Property<string>("CopyrightOwner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseCoFacilitatorIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseCode")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("CourseContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseFacilitatorIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("CourseFee")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CourseLevel")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("CourseName")
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.Property<string>("CourseObjective")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseOutlineStructure")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseType")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(15)")
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasDefaultValue("New");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DevelopmentalRoleIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DurationHours")
                        .HasColumnType("int");

                    b.Property<int?>("DurationMinutes")
                        .HasColumnType("int");

                    b.Property<string>("ECertificatePrerequisite")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ECertificateTemplateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EasSubstantiveGradeBandingIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpiredDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExternalId")
                        .HasColumnType("varchar(255)")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<Guid?>("FirstAdministratorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("JobFamily")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LearningAreaIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LearningDimensionIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LearningFrameworkIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LearningMode")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("LearningSubAreaIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MOEOfficerEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("MOEOfficerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MOEOfficerPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MaxParticipantPerClass")
                        .HasColumnType("int");

                    b.Property<int?>("MaximumPlacesPerSchool")
                        .HasColumnType("int");

                    b.Property<int?>("MinParticipantPerClass")
                        .HasColumnType("int");

                    b.Property<string>("NatureOfCourse")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<decimal?>("NotionalCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("NumOfBeginningTeacher")
                        .HasColumnType("int");

                    b.Property<int?>("NumOfHoursPerClass")
                        .HasColumnType("int");

                    b.Property<int?>("NumOfHoursPerSession")
                        .HasColumnType("int");

                    b.Property<int?>("NumOfMiddleManagement")
                        .HasColumnType("int");

                    b.Property<int?>("NumOfPlannedClass")
                        .HasColumnType("int");

                    b.Property<int?>("NumOfSchoolLeader")
                        .HasColumnType("int");

                    b.Property<int?>("NumOfSeniorOrLeadTeacher")
                        .HasColumnType("int");

                    b.Property<int?>("NumOfSessionPerClass")
                        .HasColumnType("int");

                    b.Property<string>("OtherTrainingAgencyReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnerBranchIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnerDivisionIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PDActivityType")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("PDAreaThemeCode")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("PDAreaThemeId")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("PartnerOrganisationIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PdActivityPeriods")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlaceOfWork")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50)
                        .HasDefaultValue("ApplicableForEveryone");

                    b.Property<DateTime?>("PlanningArchiveDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PlanningPublishDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PostCourseEvaluationFormId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PreCourseEvaluationFormId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PrerequisiteCourseIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PrimaryApprovingOfficerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("PublishDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PublishedContentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RegistrationMethod")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50)
                        .HasDefaultValue("RegistrationOrNomination");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000)
                        .IsUnicode(true);

                    b.Property<Guid?>("SecondAdministratorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ServiceSchemeIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255)
                        .IsUnicode(true);

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasDefaultValue("Draft");

                    b.Property<string>("SubjectAreaIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SubmittedContentDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("SubmittedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TeacherOutcomeIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeachingCourseStudyIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeachingLevels")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeachingSubjectIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TermsOfUse")
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000)
                        .IsUnicode(true);

                    b.Property<string>("ThumbnailUrl")
                        .HasColumnType("varchar(300)")
                        .HasMaxLength(300)
                        .IsUnicode(false);

                    b.Property<int?>("TotalHoursAttendWithinYear")
                        .HasColumnType("int");

                    b.Property<string>("TrackIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TrainingAgency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TraisiCourseCode")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("Version")
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("Microservice.Course.Domain.Entities.Lecture", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ClassRunId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LectureIcon")
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<string>("LectureName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.Property<int?>("Order")
                        .HasColumnType("int");

                    b.Property<Guid?>("SectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Type")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Lecture");
                });

            modelBuilder.Entity("Microservice.Course.Domain.Entities.LectureContent", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("((0))");

                    b.Property<Guid>("LectureId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MimeType")
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.Property<Guid?>("ResourceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasMaxLength(20)
                        .IsUnicode(false);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LectureContent");
                });

            modelBuilder.Entity("Microservice.Course.Domain.Entities.Registration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AdministratedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("AdministrationDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("AlternativeApprovingOfficer")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ApprovingDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ApprovingOfficer")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ClassRunChangeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ClassRunChangeRequestedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ClassRunChangeStatus")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<Guid>("ClassRunId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastStatusChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RegistrationType")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasDefaultValue("Application");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasDefaultValue("PendingConfirmation");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("WithdrawalRequestDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("WithdrawalStatus")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Registration");
                });

            modelBuilder.Entity("Microservice.Course.Domain.Entities.Section", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ClassRunId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CreditsAward")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("Order")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Section");
                });

            modelBuilder.Entity("Microservice.Course.Domain.Entities.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ClassRunId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("RescheduleEndDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RescheduleStartDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SessionTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Venue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Session");
                });
#pragma warning restore 612, 618
        }
    }
}
