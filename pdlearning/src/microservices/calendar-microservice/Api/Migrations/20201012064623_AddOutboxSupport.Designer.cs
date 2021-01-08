﻿// <auto-generated />
using System;
using Microservice.Calendar.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microservice.Calendar.Migrations
{
    [DbContext(typeof(CalendarDbContext))]
    [Migration("20201012064623_AddOutboxSupport")]
    partial class AddOutboxSupport
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

            modelBuilder.Entity("Microservice.Calendar.Domain.Entities.EventEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsAllDay")
                        .HasColumnType("bit");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("varchar(21)")
                        .HasMaxLength(21)
                        .IsUnicode(false);

                    b.Property<Guid?>("SourceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(2000)")
                        .HasMaxLength(2000)
                        .IsUnicode(true);

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Microservice.Calendar.Domain.Entities.UserEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ChangedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("EventId", "UserId")
                        .IsUnique();

                    b.ToTable("UserEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
