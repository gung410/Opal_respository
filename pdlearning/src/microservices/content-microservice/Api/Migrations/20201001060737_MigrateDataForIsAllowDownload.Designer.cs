﻿// <auto-generated />
using System;
using Microservice.Content.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Content.Migrations
{
    [DbContext(typeof(ContentDbContext))]
    [Migration("20201001060737_MigrateDataForIsAllowDownload")]
    partial class MigrateDataForIsAllowDownload
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

                    b.HasIndex("Status");

                    b.HasIndex("CreatedDate", "Status");

                    b.ToTable("OutboxMessages");
                });

            modelBuilder.Entity("Microservice.Content.Domain.Entities.AccessRight", b =>
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

            modelBuilder.Entity("Microservice.Content.Domain.Entities.AttributionElement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255)
                        .IsUnicode(true);

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DigitalContentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LicenseType")
                        .IsRequired()
                        .HasColumnType("varchar(36)")
                        .HasMaxLength(36)
                        .IsUnicode(false);

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500)
                        .IsUnicode(true);

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255)
                        .IsUnicode(true);

                    b.HasKey("Id");

                    b.ToTable("AttributionElements");
                });

            modelBuilder.Entity("Microservice.Content.Domain.Entities.Comment", b =>
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

            modelBuilder.Entity("Microservice.Content.Domain.Entities.DigitalContent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AcknowledgementAndCredit")
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000)
                        .IsUnicode(true);

                    b.Property<Guid?>("AlternativeApprovingOfficerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ArchiveDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ArchivedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("AverageRating")
                        .HasColumnType("float")
                        .IsUnicode(false);

                    b.Property<Guid>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Copyright")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(true);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpiredDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("varchar(255)")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<bool>("IsAllowDownload")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsAllowModification")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsAllowReusable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsArchived")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("LicenseTerritory")
                        .IsRequired()
                        .HasColumnType("varchar(30)")
                        .HasMaxLength(30)
                        .IsUnicode(false);

                    b.Property<string>("LicenseType")
                        .IsRequired()
                        .HasColumnType("varchar(36)")
                        .HasMaxLength(36)
                        .IsUnicode(false);

                    b.Property<Guid>("OriginalObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Ownership")
                        .IsRequired()
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40)
                        .IsUnicode(false);

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PrimaryApprovingOfficerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Publisher")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(true);

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000)
                        .IsUnicode(true);

                    b.Property<string>("RepositoryName")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(true);

                    b.Property<int>("ReviewCount")
                        .HasColumnType("int")
                        .IsUnicode(false);

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255)
                        .IsUnicode(true);

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("varchar(28)")
                        .HasMaxLength(28)
                        .IsUnicode(false);

                    b.Property<DateTime?>("SubmitDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TermsOfUse")
                        .HasColumnType("nvarchar(4000)")
                        .HasMaxLength(4000)
                        .IsUnicode(true);

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(25)")
                        .HasMaxLength(25)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("DigitalContents");

                    b.HasDiscriminator<string>("Discriminator").HasValue("DigitalContent");
                });

            modelBuilder.Entity("Microservice.Content.Versioning.Entities.VersionTracking", b =>
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

                    b.ToTable("VersionTrackings");
                });

            modelBuilder.Entity("Microservice.Content.Domain.Entities.LearningContent", b =>
                {
                    b.HasBaseType("Microservice.Content.Domain.Entities.DigitalContent");

                    b.Property<string>("HtmlContent")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("LearningContent");
                });

            modelBuilder.Entity("Microservice.Content.Domain.Entities.UploadedContent", b =>
                {
                    b.HasBaseType("Microservice.Content.Domain.Entities.DigitalContent");

                    b.Property<string>("FileExtension")
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<string>("FileLocation")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<double>("FileSize")
                        .HasColumnType("float");

                    b.Property<string>("FileType")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasDiscriminator().HasValue("UploadedContent");
                });
#pragma warning restore 612, 618
        }
    }
}
