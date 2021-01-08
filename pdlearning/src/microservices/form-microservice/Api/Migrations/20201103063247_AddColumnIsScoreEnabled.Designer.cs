﻿// <auto-generated />
using System;
using Microservice.Form.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Form.Migrations
{
    [DbContext(typeof(FormDbContext))]
    [Migration("20201103063247_AddColumnIsScoreEnabled")]
    partial class AddColumnIsScoreEnabled
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.Department", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnName("DepartmentID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.DepartmentType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentTypeId")
                        .HasColumnName("DepartmentTypeID")
                        .HasColumnType("int");

                    b.Property<string>("ExtId")
                        .HasColumnName("ExtID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DepartmentTypes");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.DepartmentTypeDepartment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnName("DepartmentID")
                        .HasColumnType("int");

                    b.Property<int>("DepartmentTypeId")
                        .HasColumnName("DepartmentTypeID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("DepartmentTypeDepartments");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.HierarchyDepartment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnName("DepartmentID")
                        .HasColumnType("int");

                    b.Property<int>("HierarchyDepartmentId")
                        .HasColumnName("HDID")
                        .HasColumnType("int");

                    b.Property<int?>("ParentId")
                        .HasColumnName("ParentID")
                        .HasColumnType("int");

                    b.Property<string>("Path")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.ToTable("HierarchyDepartments");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnName("DepartmentID")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<int>("OriginalUserId")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Conexus.Opal.OutboxPattern.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Exchange")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("FailReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MessageData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PreparedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("ReadyToDelete")
                        .IsConcurrencyToken()
                        .HasColumnType("bit");

                    b.Property<string>("RoutingKey")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<int>("SendTimes")
                        .HasColumnType("int");

                    b.Property<string>("SourceIp")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Status")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasColumnType("varchar(19)")
                        .HasMaxLength(19)
                        .IsUnicode(false);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Status", "CreatedDate");

                    b.ToTable("OutboxMessages");
                });

            modelBuilder.Entity("Microservice.Form.Domain.Entities.AccessRight", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("AccessRights");
                });

            modelBuilder.Entity("Microservice.Form.Domain.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Microservice.Form.Domain.Entities.Form", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AlternativeApprovingOfficerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AnswerFeedbackDisplayOption")
                        .HasColumnType("varchar(30)");

                    b.Property<short?>("AttemptToShowFeedback")
                        .HasColumnType("smallint");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("InSecondTimeLimit")
                        .HasColumnType("int");

                    b.Property<bool?>("IsAllowedDisplayPollResult")
                        .HasColumnType("bit");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool?>("IsShowFreeTextQuestionInPoll")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsSurveyTemplate")
                        .HasColumnType("bit");

                    b.Property<short?>("MaxAttempt")
                        .HasColumnType("smallint");

                    b.Property<Guid>("OriginalObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<short?>("PassingMarkPercentage")
                        .HasColumnType("smallint");

                    b.Property<int?>("PassingMarkScore")
                        .HasColumnType("int");

                    b.Property<Guid?>("PrimaryApprovingOfficerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("RandomizedQuestions")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool?>("ShowQuizSummary")
                        .HasColumnType("bit");

                    b.Property<string>("SqRatingType")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<DateTime?>("SubmitDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SurveyType")
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy", "IsDeleted", "CreatedDate");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("Microservice.Form.Domain.Entities.FormAnswer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssignmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("Attempt")
                        .HasColumnType("smallint");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ClassRunId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("FormId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FormMetaData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid?>("MyCourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ResourceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Score")
                        .HasColumnType("float");

                    b.Property<double?>("ScorePercentage")
                        .HasColumnType("float");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("SubmitDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId", "IsDeleted", "CreatedDate");

                    b.HasIndex("ClassRunId", "IsDeleted", "CreatedDate");

                    b.HasIndex("CreatedBy", "IsDeleted", "CreatedDate");

                    b.HasIndex("FormId", "IsDeleted", "CreatedDate");

                    b.HasIndex("OwnerId", "IsDeleted", "CreatedDate");

                    b.HasIndex("ResourceId", "IsDeleted", "CreatedDate");

                    b.ToTable("FormAnswers");
                });

            modelBuilder.Entity("Microservice.Form.Domain.Entities.FormQuestion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("FormId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("FormSectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsScoreEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<bool?>("IsShareIdentityQuestion")
                        .HasColumnType("bit");

                    b.Property<int?>("MinorPriority")
                        .HasColumnType("int");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Priority")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Question_AnswerExplanatoryNote")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_CorrectAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_FeedbackCorrectAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_FeedbackWrongAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_Hint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Question_IsSurveyTemplateQuestion")
                        .HasColumnType("bit");

                    b.Property<int?>("Question_Level")
                        .HasColumnType("int");

                    b.Property<Guid?>("Question_NextQuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Question_Options")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_Title")
                        .HasColumnType("NTEXT")
                        .HasMaxLength(20000);

                    b.Property<string>("Question_Type")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<bool?>("RandomizedOptions")
                        .HasColumnType("bit");

                    b.Property<double?>("Score")
                        .HasColumnType("float");

                    b.Property<bool?>("ShowFeedBackAfterAnswer")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("NTEXT")
                        .HasMaxLength(20000);

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy", "IsDeleted", "CreatedDate");

                    b.HasIndex("FormId", "IsDeleted", "CreatedDate");

                    b.ToTable("FormQuestions");
                });

            modelBuilder.Entity("Microservice.Form.Domain.Entities.FormQuestionAnswer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AnswerFeedback")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AnswerValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("FormAnswerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FormQuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("MaxScore")
                        .HasColumnType("float");

                    b.Property<double?>("Score")
                        .HasColumnType("float");

                    b.Property<Guid?>("ScoredBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("SpentTimeInSeconds")
                        .HasColumnType("int");

                    b.Property<DateTime?>("SubmittedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy", "CreatedDate");

                    b.HasIndex("FormAnswerId", "CreatedDate");

                    b.HasIndex("ScoredBy", "CreatedDate");

                    b.HasIndex("CreatedBy", "FormAnswerId", "CreatedDate");

                    b.ToTable("FormQuestionAnswers");
                });

            modelBuilder.Entity("Microservice.Form.Domain.Entities.FormQuestionAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ExternalId")
                        .HasColumnType("VARCHAR(255)")
                        .HasMaxLength(255);

                    b.Property<string>("FileExtension")
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("FileLocation")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("FileType")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<Guid>("FormQuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("FormQuestionAttachments");
                });

            modelBuilder.Entity("Microservice.Form.Domain.Entities.FormSection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AdditionalDescription")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("varchar(255)");

                    b.Property<Guid>("FormId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("MainDescription")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<Guid?>("NextQuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy", "IsDeleted", "CreatedDate");

                    b.HasIndex("FormId", "IsDeleted", "CreatedDate");

                    b.ToTable("FormSections");
                });

            modelBuilder.Entity("Microservice.Form.Domain.Entities.SharedQuestion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Question_AnswerExplanatoryNote")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_CorrectAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_FeedbackCorrectAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_FeedbackWrongAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_Hint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Question_IsSurveyTemplateQuestion")
                        .HasColumnType("bit");

                    b.Property<int?>("Question_Level")
                        .HasColumnType("int");

                    b.Property<Guid?>("Question_NextQuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Question_Options")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question_Title")
                        .HasColumnType("NTEXT")
                        .HasMaxLength(20000);

                    b.Property<string>("Question_Type")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.HasKey("Id");

                    b.ToTable("SharedQuestions");
                });

            modelBuilder.Entity("Microservice.Form.Versioning.Entities.VersionTracking", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("CanRollback")
                        .HasColumnType("bit");

                    b.Property<Guid>("ChangedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MajorVersion")
                        .HasColumnType("int");

                    b.Property<int>("MinorVersion")
                        .HasColumnType("int");

                    b.Property<int>("ObjectType")
                        .HasColumnType("int");

                    b.Property<Guid>("OriginalObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RevertObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SchemaVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OriginalObjectId", "CreatedDate");

                    b.HasIndex("OriginalObjectId", "MajorVersion", "CreatedDate");

                    b.ToTable("VersionTrackings");
                });
#pragma warning restore 612, 618
        }
    }
}
