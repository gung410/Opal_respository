﻿// <auto-generated />
using System;
using Microservice.LnaForm.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.LnaForm.Migrations
{
    [DbContext(typeof(LnaFormDbContext))]
    [Migration("20201118104528_RemoveSurveyType")]
    partial class RemoveSurveyType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.Department", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID");

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.DepartmentType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentTypeId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentTypeID");

                    b.Property<string>("ExtId")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExtID");

                    b.HasKey("Id");

                    b.ToTable("DepartmentTypes");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.DepartmentTypeDepartment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID");

                    b.Property<int>("DepartmentTypeId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentTypeID");

                    b.HasKey("Id");

                    b.ToTable("DepartmentTypeDepartments");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.HierarchyDepartment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID");

                    b.Property<int>("HierarchyDepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("HDID");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int")
                        .HasColumnName("ParentID");

                    b.Property<string>("Path")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.ToTable("HierarchyDepartments");
                });

            modelBuilder.Entity("Conexus.Opal.AccessControl.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentID");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("OriginalUserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

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
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

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
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("SendTimes")
                        .HasColumnType("int");

                    b.Property<string>("SourceIp")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Status")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(19)
                        .IsUnicode(false)
                        .HasColumnType("varchar(19)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("UserId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Status", "CreatedDate");

                    b.ToTable("OutboxMessages");
                });

            modelBuilder.Entity("Microservice.LnaForm.Domain.Entities.AccessRight", b =>
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

            modelBuilder.Entity("Microservice.LnaForm.Domain.Entities.Comment", b =>
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

            modelBuilder.Entity("Microservice.LnaForm.Domain.Entities.Form", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ArchiveDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ArchivedBy")
                        .HasColumnType("uniqueidentifier");

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

                    b.Property<DateTime?>("FormRemindDueDate")
                        .HasColumnType("datetime2");

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

                    b.Property<bool?>("IsStandalone")
                        .HasColumnType("bit");

                    b.Property<Guid>("OriginalObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RemindBeforeDays")
                        .HasColumnType("int");

                    b.Property<bool?>("ShowQuizSummary")
                        .HasColumnType("bit");

                    b.Property<string>("SqRatingType")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("StandaloneMode")
                        .HasColumnType("varchar(30)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<DateTime?>("SubmitDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy", "IsDeleted", "CreatedDate");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("Microservice.LnaForm.Domain.Entities.FormAnswer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("Attempt")
                        .HasColumnType("smallint");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

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

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ResourceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("SubmitDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy", "IsDeleted", "CreatedDate");

                    b.HasIndex("FormId", "IsDeleted", "CreatedDate");

                    b.HasIndex("OwnerId", "IsDeleted", "CreatedDate");

                    b.HasIndex("ResourceId", "IsDeleted", "CreatedDate");

                    b.ToTable("FormAnswers");
                });

            modelBuilder.Entity("Microservice.LnaForm.Domain.Entities.FormParticipant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("AssignedDate")
                        .HasColumnType("datetime2");

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

                    b.Property<Guid>("FormOriginalObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsStarted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)")
                        .HasDefaultValue("NotStarted");

                    b.Property<DateTime?>("SubmittedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("FormParticipant");
                });

            modelBuilder.Entity("Microservice.LnaForm.Domain.Entities.FormQuestion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CorrectAnswer")
                        .HasColumnType("nvarchar(max)");

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

                    b.Property<bool?>("IsSurveyTemplateQuestion")
                        .HasColumnType("bit");

                    b.Property<int?>("MinorPriority")
                        .HasColumnType("int");

                    b.Property<Guid?>("NextQuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Options")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Priority")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("QuestionType")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Title")
                        .HasMaxLength(20000)
                        .HasColumnType("NTEXT");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy", "IsDeleted", "CreatedDate");

                    b.HasIndex("FormId", "IsDeleted", "CreatedDate");

                    b.ToTable("FormQuestions");
                });

            modelBuilder.Entity("Microservice.LnaForm.Domain.Entities.FormQuestionAnswer", b =>
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

                    b.Property<int?>("SpentTimeInSeconds")
                        .HasColumnType("int");

                    b.Property<DateTime?>("SubmittedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreatedBy", "CreatedDate");

                    b.HasIndex("FormAnswerId", "CreatedDate");

                    b.HasIndex("CreatedBy", "FormAnswerId", "CreatedDate");

                    b.ToTable("FormQuestionAnswers");
                });

            modelBuilder.Entity("Microservice.LnaForm.Domain.Entities.FormQuestionAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ExternalId")
                        .HasMaxLength(255)
                        .HasColumnType("VARCHAR(255)");

                    b.Property<string>("FileExtension")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("FileLocation")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("FileName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FileType")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("FormQuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("FormQuestionAttachments");
                });

            modelBuilder.Entity("Microservice.LnaForm.Domain.Entities.FormSection", b =>
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

            modelBuilder.Entity("Microservice.LnaForm.Versioning.Entities.VersionTracking", b =>
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
